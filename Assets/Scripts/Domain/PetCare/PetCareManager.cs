using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Master.Domain.GameEvents;
using Master.Domain.Shop;
using Master.Domain.PetCare.Log;
using Master.Auxiliar;
using Master.Domain.Connection;
using System.Diagnostics;

namespace Master.Domain.PetCare
{
    public class PetCareManager : IPetCareManager
    {
        private Mutex _mutex = new Mutex();
        private IChatBot _chatBot;
        private IPetCareRepository _petCareRepository;
        private IPetCareLogManager _petCareLogManager;
        private IScoreManager _scoreManager;
        private IEconomyManager _economyManager;

        // Update and Simulation
        public float updateIntervalBTree { get; private set; }
        public DateTime nextIterationStartTime { get; private set; }

        // Attributes Ranges
        public List<AttributeState> glycemiaRangeStates { get; private set; }
        public List<AttributeState> energyRangeStates { get; private set; }
        public List<AttributeState> hungerRangeStates { get; private set; }

        // Minimum and maximum attributes values
        public int minGlycemiaValue { get; private set; }
        public int minEnergyValue { get; private set; }
        public int minHungerValue { get; private set; }
        public int maxGlycemiaValue { get; private set; }
        public int maxEnergyValue { get; private set; }
        public int maxHungerValue { get; private set; }
        public int initialGlycemiaValue { get; private set; }
        public int initialEnergyValue { get; private set; }
        public int initialHungerValue { get; private set; }

        // Attributes values
        public int glycemiaValue { get; private set; }
        public int energyValue { get; private set; }
        public int hungerValue { get; private set; }

        private bool _isStashedGlycemiaActive = false;
        private bool _isStashedEnergyActive = false;
        private bool _isStashedHungerActive = false;

        private int _stashedGlycemiaValue = 0;
        private int _stashedEnergyValue = 0;
        private int _stashedHungerValue = 0;

        // Last time actions used
        public DateTime insulinCooldownEndTime { get; private set; }
        public DateTime foodCooldownEndTime { get; private set; }
        public DateTime exerciseCooldownEndTime { get; private set; }

        public DateTime insulinEffectsEndTime { get; private set; }
        public DateTime foodEffectsEndTime { get; private set; }
        public DateTime exerciseEffectsEndTime { get; private set; }

        // Actions in CD?
        public bool isInsulinActionInCD { get; set; }
        public bool isExerciseActionInCD { get; set; }
        public bool isFoodActionInCD { get; set; }

        // Actions effects active?
        public bool isInsulinEffectActive { get; set; }
        public bool isExerciseEffectActive { get; set; }
        public bool isFoodEffectActive { get; set; }

        // Actions settings
        public float timeCDActions { get; private set; }
        public float timeEffectActions { get; private set; }

        public PetCareManager(IChatBot chatBot, IPetCareRepository petCareRepository, IPetCareLogManager petCareLogManager, IScoreManager scoreManager, IEconomyManager economyManager, IConnectionManager connectionManager)
        {
            _petCareRepository = petCareRepository;
            _chatBot = chatBot;
            _petCareLogManager = petCareLogManager;
            _scoreManager = scoreManager;
            _economyManager = economyManager;

            updateIntervalBTree = 300; // 5 minutos 300

            minGlycemiaValue = 20;
            minEnergyValue = 0;
            minHungerValue = 0;
            maxGlycemiaValue = 250;
            maxEnergyValue = 100;
            maxHungerValue = 100;
            initialGlycemiaValue = 120;
            initialEnergyValue = 50;
            initialHungerValue = 50;

            nextIterationStartTime = _petCareRepository.LoadNextIterationStartTime();

            timeCDActions = 3600; // 60 minutos 3600
            timeEffectActions = 1800; // 30 minutos 1800

            glycemiaValue = _petCareRepository.LoadGlycemia();
            energyValue = _petCareRepository.LoadEnergy();
            hungerValue = _petCareRepository.LoadHunger();

            InitializeActions();
            InitializeAttributesRangeStates();
        }

        public void ExecuteAttributesBTree()
        {
            AttributeUpdateIntervalInfo currentIntervalInfo = new AttributeUpdateIntervalInfo
                (
                    nextIterationStartTime,
                    glycemiaValue,
                    energyValue,
                    hungerValue,
                    isInsulinEffectActive,
                    isExerciseEffectActive,
                    isFoodEffectActive
                );

            GameEvents_PetCare.OnExecuteAttributesBTree?.Invoke(currentIntervalInfo);
        }

        private void InitializeAttributesRangeStates()
        {
            glycemiaRangeStates = new List<AttributeState>();
            glycemiaRangeStates.Add(new AttributeState(211, 250, AttributeRangeValue.BadHigh));
            glycemiaRangeStates.Add(new AttributeState(151, 210, AttributeRangeValue.IntermediateHigh));
            glycemiaRangeStates.Add(new AttributeState(90, 150, AttributeRangeValue.Good));
            glycemiaRangeStates.Add(new AttributeState(61, 89, AttributeRangeValue.IntermediateLow));
            glycemiaRangeStates.Add(new AttributeState(20, 60, AttributeRangeValue.BadLow));

            energyRangeStates = new List<AttributeState>();
            energyRangeStates.Add(new AttributeState(86, 100, AttributeRangeValue.BadHigh));
            energyRangeStates.Add(new AttributeState(71, 85, AttributeRangeValue.IntermediateHigh));
            energyRangeStates.Add(new AttributeState(30, 70, AttributeRangeValue.Good));
            energyRangeStates.Add(new AttributeState(15, 29, AttributeRangeValue.IntermediateLow));
            energyRangeStates.Add(new AttributeState(0, 14, AttributeRangeValue.BadLow));

            hungerRangeStates = new List<AttributeState>();
            hungerRangeStates.Add(new AttributeState(86, 100, AttributeRangeValue.BadHigh));
            hungerRangeStates.Add(new AttributeState(71, 85, AttributeRangeValue.IntermediateHigh));
            hungerRangeStates.Add(new AttributeState(30, 70, AttributeRangeValue.Good));
            hungerRangeStates.Add(new AttributeState(15, 29, AttributeRangeValue.IntermediateLow));
            hungerRangeStates.Add(new AttributeState(0, 14, AttributeRangeValue.BadLow));
        }

        private void InitializeActions()
        {
            DateTime now = DateTime.Now;

            isInsulinActionInCD = false;
            isFoodActionInCD = false;
            isExerciseActionInCD = false;

            // Insulin
            insulinCooldownEndTime = _petCareRepository.LoadInsulinCooldownEndTime();
            _petCareRepository.SaveInsulinCooldownEndTime(now.AddSeconds(-1));
            if (now < insulinCooldownEndTime)
            {
                float currentCDInsulin = (float)(insulinCooldownEndTime - now).TotalSeconds;
                _ = ActivateCDInsulin(currentCDInsulin);
            }

            insulinEffectsEndTime = _petCareRepository.LoadInsulinEffectsEndTime();
            if (now < insulinEffectsEndTime)
            {
                float currentTimeEffectsInsulin = (float)(insulinEffectsEndTime - now).TotalSeconds;
                _ = ActivateInsulinEffect(currentTimeEffectsInsulin);
            }
            else
            {
                _petCareRepository.SaveInsulinEffectsEndTime(now.AddSeconds(-1));
            }

                // Exercise
                exerciseCooldownEndTime = _petCareRepository.LoadExerciseCooldownEndTime();
            _petCareRepository.SaveExerciseCooldownEndTime(now.AddSeconds(-1));
            if (now < exerciseCooldownEndTime)
            {
                float currentCDExercise = (float)(exerciseCooldownEndTime - now).TotalSeconds;
                _ = ActivateCDExercise(currentCDExercise);
            }

            exerciseEffectsEndTime = _petCareRepository.LoadExerciseEffectsEndTime();
            if (now < exerciseEffectsEndTime)
            {
                float currentTimeEffectsExercise = (float)(exerciseEffectsEndTime - now).TotalSeconds;
                _ = ActivateExerciseEffect(currentTimeEffectsExercise);
            }
            else
            {
                _petCareRepository.SaveExerciseEffectsEndTime(now.AddSeconds(-1));
            }

            // Food
            foodCooldownEndTime = _petCareRepository.LoadFoodCooldownEndTime();
            _petCareRepository.SaveFoodCooldownEndTime(now.AddSeconds(-1));
            if (now < foodCooldownEndTime)
            {
                float currentCDFood = (float)(foodCooldownEndTime - now).TotalSeconds;
                _ = ActivateCDFood(currentCDFood);
            }

            foodEffectsEndTime = _petCareRepository.LoadFoodEffectsEndTime();
            if (now < foodEffectsEndTime)
            {
                float currentTimeEffectsFood = (float)(foodEffectsEndTime - now).TotalSeconds;
                _ = ActivateFoodEffect(currentTimeEffectsFood);
            }
            else
            {
                _petCareRepository.SaveFoodEffectsEndTime(now.AddSeconds(-1));
            }
        }

        public void RestartGlycemia(DateTime? currentDateTime)
        {
            glycemiaValue = UtilityFunctions.Clamp(initialGlycemiaValue, minGlycemiaValue, maxGlycemiaValue);

            _petCareRepository.SaveGlycemia(initialGlycemiaValue);
            GameEvents_PetCare.OnModifyGlycemiaUI?.Invoke(initialGlycemiaValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Glycemia, new AttributeLog(currentDateTime, initialGlycemiaValue));
        }

        public void RestartEnergy(DateTime? currentDateTime)
        {
            energyValue = UtilityFunctions.Clamp(initialEnergyValue, minEnergyValue, maxEnergyValue);

            _petCareRepository.SaveEnergy(initialEnergyValue);
            GameEvents_PetCare.OnModifyEnergyUI?.Invoke(initialEnergyValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Energy, new AttributeLog(currentDateTime, initialEnergyValue));
        }
        public void RestartHunger(DateTime? currentDateTime)
        {
            hungerValue = UtilityFunctions.Clamp(initialHungerValue, minHungerValue, maxHungerValue);

            _petCareRepository.SaveHunger(initialHungerValue);
            GameEvents_PetCare.OnModifyHungerUI?.Invoke(initialHungerValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Hunger, new AttributeLog(currentDateTime, initialHungerValue));
        }

        public void StartStashGlycemia()
        {
            if (_isStashedGlycemiaActive)
                return;

            _isStashedGlycemiaActive = true;
            _stashedGlycemiaValue = 0;
        }

        public void ApplyStashedGlycemia(DateTime? currentDateTime)
        {
            _isStashedGlycemiaActive = false;

            UnityEngine.Debug.Log($"Glycemia total = {_stashedGlycemiaValue}");
            ModifyGlycemia(_stashedGlycemiaValue, currentDateTime);
        }

        public void ModifyGlycemia(int value, DateTime? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar glucemia
                if (_isStashedGlycemiaActive)
                {
                    _stashedGlycemiaValue += value;
                    return;
                }

                glycemiaValue = UtilityFunctions.Clamp(glycemiaValue + value, minGlycemiaValue, maxGlycemiaValue);
                _petCareRepository.SaveGlycemia(glycemiaValue);

                // Actualizar interfaz
                GameEvents_PetCare.OnModifyGlycemiaUI?.Invoke(glycemiaValue);

                if (isCalledByAction == false)
                {
                    // Añadir al log de atributos
                    _petCareLogManager.AddAttributeLog(AttributeType.Glycemia, new AttributeLog(currentDateTime, glycemiaValue));

                    // Sumar puntos y sumar monedas
                    if (IsGlycemiaInRange(AttributeRangeValue.BadHigh, glycemiaValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo de la glucemia");
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.IntermediateHigh, glycemiaValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular de la glucemia");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.Good, glycemiaValue))
                    {
                        int addedScore = 3;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control bueno de la glucemia");
                        _economyManager.AddStashedCoins(2);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.IntermediateLow, glycemiaValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular de la glucemia");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.BadLow, glycemiaValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo de la glucemia");
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void StartStashEnergy()
        {
            if (_isStashedEnergyActive)
                return;

            _isStashedEnergyActive = true;
            _stashedEnergyValue = 0;
        }

        public void ApplyStashedEnergy(DateTime? currentDateTime)
        {
            _isStashedEnergyActive = false;

            UnityEngine.Debug.Log($"Energy total = {_stashedEnergyValue}");
            ModifyEnergy(_stashedEnergyValue, currentDateTime);
        }

        public void ModifyEnergy(int value, DateTime? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar energía
                if (_isStashedEnergyActive)
                {
                    _stashedEnergyValue += value;
                    return;
                }

                energyValue = UtilityFunctions.Clamp(energyValue + value, minEnergyValue, maxEnergyValue);
                _petCareRepository.SaveEnergy(energyValue);

                // Actualizar interfaz y añadir al log de atributos
                GameEvents_PetCare.OnModifyEnergyUI?.Invoke(energyValue);

                if (isCalledByAction == false)
                {
                    // Añadir al log de atributos
                    _petCareLogManager.AddAttributeLog(AttributeType.Energy, new AttributeLog(currentDateTime, energyValue));

                    // Sumar puntos y sumar monedas
                    if (IsEnergyInRange(AttributeRangeValue.BadHigh, energyValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo de la energía");
                    }
                    else if (IsEnergyInRange(AttributeRangeValue.IntermediateHigh, energyValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular de la energía");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsEnergyInRange(AttributeRangeValue.Good, energyValue))
                    {
                        int addedScore = 3;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control bueno de la energía");
                        _economyManager.AddStashedCoins(2);
                    }
                    else if (IsEnergyInRange(AttributeRangeValue.IntermediateLow, energyValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular de la energía");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsEnergyInRange(AttributeRangeValue.BadLow, energyValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo de la energía");
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void StartStashHunger()
        {
            if (_isStashedHungerActive)
                return;

            _isStashedHungerActive = true;
            _stashedHungerValue = 0;
        }

        public void ApplyStashedHunger(DateTime? currentDateTime)
        {
            _isStashedHungerActive = false;

            UnityEngine.Debug.Log($"Hunger total = {_stashedHungerValue}");
            ModifyHunger(_stashedHungerValue, currentDateTime);
        }

        public void ModifyHunger(int value, DateTime? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar hambre
                if (_isStashedHungerActive)
                {
                    _stashedHungerValue += value;
                    return;
                }

                hungerValue = UtilityFunctions.Clamp(hungerValue + value, minHungerValue, maxHungerValue);
                _petCareRepository.SaveHunger(hungerValue);

                // Actualizar interfaz y añadir al log de atributos
                GameEvents_PetCare.OnModifyHungerUI?.Invoke(hungerValue);

                if (isCalledByAction == false)
                {
                    // Añadir al log de atributos
                    _petCareLogManager.AddAttributeLog(AttributeType.Hunger, new AttributeLog(currentDateTime, hungerValue));

                    // Sumar puntos y sumar monedas
                    if (IsHungerInRange(AttributeRangeValue.BadHigh, hungerValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo del hambre");
                    }
                    else if (IsHungerInRange(AttributeRangeValue.IntermediateHigh, hungerValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular del hambre");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.Good, hungerValue))
                    {
                        int addedScore = 3;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control bueno del hambre");
                        _economyManager.AddStashedCoins(2);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.IntermediateLow, hungerValue))
                    {
                        int addedScore = 1;
                        _scoreManager.AddScore(addedScore, currentDateTime, "control regular del hambre");
                        _economyManager.AddStashedCoins(1);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.BadLow, hungerValue))
                    {
                        int substractedScore = 2;
                        _scoreManager.SubstractScore(substractedScore, currentDateTime, "control malo del hambre");
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool IsGlycemiaInRange(AttributeRangeValue attributeStateRequested, int currentGlycemiaValue)
        {
            foreach (AttributeState currentState in glycemiaRangeStates)
            {
                if (currentGlycemiaValue >= currentState.minValue && currentGlycemiaValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsEnergyInRange(AttributeRangeValue attributeStateRequested, int currentEnergyValue)
        {
            foreach (AttributeState currentState in energyRangeStates)
            {
                if (currentEnergyValue >= currentState.minValue && currentEnergyValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsHungerInRange(AttributeRangeValue attributeStateRequested, int currentHungerValue)
        {
            foreach (AttributeState currentState in hungerRangeStates)
            {
                if (currentHungerValue >= currentState.minValue && currentHungerValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public void ActivateInsulinAction(int value)
        {
            insulinEffectsEndTime = DateTime.Now.AddSeconds(timeEffectActions);
            _petCareRepository.SaveInsulinEffectsEndTime(insulinEffectsEndTime);

            int affectedGlycemia = value * -85;
            ModifyGlycemia(affectedGlycemia, DateTime.Now, true);

            // Se guarda la información para la gráfica.
            string informationLog = $"";
            if (affectedGlycemia == 1)
            {
                informationLog = $"{value} unidadad.";
            }
            else
            {
                informationLog = $"{value} unidades.";
            }
            _petCareLogManager.AddActionLog(ActionType.Insulin, new ActionLog(DateTime.Now, informationLog));

            _ = ActivateCDInsulin(timeCDActions);
            _ = ActivateInsulinEffect(timeEffectActions);
        }

        private async Task ActivateCDInsulin(float time)
        {
            isInsulinActionInCD = true;
            insulinCooldownEndTime = DateTime.Now.AddSeconds(time);
            _petCareRepository.SaveInsulinCooldownEndTime(insulinCooldownEndTime);
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Insulin);
            await Task.Delay((int)(time * 1000));
            DeactivateInsulinActionCD();
        }

        public void DeactivateInsulinActionCD()
        {
            isInsulinActionInCD = false;
            insulinCooldownEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveInsulinCooldownEndTime(insulinCooldownEndTime);
            GameEvents_PetCare.OnFinishTimerCD(ActionType.Insulin);
        }

        private async Task ActivateInsulinEffect(float time)
        {
            isInsulinEffectActive = true;
            await Task.Delay((int)(time * 1000));
            DeactivateInsulinEffect();
        }

        public void DeactivateInsulinEffect()
        {
            insulinEffectsEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveInsulinEffectsEndTime(insulinEffectsEndTime);
            isInsulinEffectActive = false;
        }

        public void ActivateExerciseAction(string intensity)
        {
            exerciseEffectsEndTime = DateTime.Now.AddSeconds(timeEffectActions);
            _petCareRepository.SaveExerciseEffectsEndTime(exerciseEffectsEndTime);

            switch (intensity)
            {
                case "Intensidad baja":
                    ModifyGlycemia(-30, DateTime.Now, true);
                    ModifyEnergy(-15, DateTime.Now, true);
                    ModifyHunger(10, DateTime.Now, true);
                    break;
                case "Intensidad media":
                    ModifyGlycemia(-60, DateTime.Now, true);
                    ModifyEnergy(-35, DateTime.Now, true);
                    ModifyHunger(15, DateTime.Now, true);
                    break;
                case "Intensidad alta":
                    ModifyGlycemia(-90, DateTime.Now, true);
                    ModifyEnergy(-50, DateTime.Now, true);
                    ModifyHunger(20, DateTime.Now, true);
                    break;
            }

            // Se guarda la información para la gráfica.
            string informationLog = $"{intensity}.";
            _petCareLogManager.AddActionLog(ActionType.Exercise, new ActionLog(DateTime.Now, informationLog));

            _ = ActivateCDExercise(timeCDActions);
            _ = ActivateExerciseEffect(timeEffectActions);
        }

        private async Task ActivateCDExercise(float time)
        {
            isExerciseActionInCD = true;
            exerciseCooldownEndTime = DateTime.Now.AddSeconds(time);
            _petCareRepository.SaveExerciseCooldownEndTime(exerciseCooldownEndTime);
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Exercise);
            await Task.Delay((int)(time * 1000));
            DeactivateExerciseActionCD();
        }

        public void DeactivateExerciseActionCD()
        {
            isExerciseActionInCD = false;
            exerciseCooldownEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveExerciseCooldownEndTime(exerciseCooldownEndTime);
            GameEvents_PetCare.OnFinishTimerCD(ActionType.Exercise);
        }

        private async Task ActivateExerciseEffect(float time)
        {
            isExerciseEffectActive = true;
            await Task.Delay((int)(time * 1000));
            DeactivateExerciseEffect();
        }

        public void DeactivateExerciseEffect()
        {
            exerciseEffectsEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveExerciseEffectsEndTime(exerciseEffectsEndTime);
            isExerciseEffectActive = false;
        }

        public void ActivateFoodAction(float ration, string food)
        {
            foodEffectsEndTime = DateTime.Now.AddSeconds(timeEffectActions);
            _petCareRepository.SaveFoodEffectsEndTime(foodEffectsEndTime);

            float affectedGlycemia = ration * 34;
            ModifyGlycemia((int)affectedGlycemia, DateTime.Now, true);

            if (ration > 0 && ration <= 2)
            {
                ModifyHunger(-20, DateTime.Now, true);
                ModifyEnergy(10, DateTime.Now, true);
            }
            else if (ration > 2 && ration <= 4.5)
            {
                ModifyHunger(-40, DateTime.Now, true);
                ModifyEnergy(15, DateTime.Now, true);
            }
            else if (ration > 4.5)
            {
                ModifyHunger(-60, DateTime.Now, true);
                ModifyEnergy(20, DateTime.Now, true);
            }

            // Se guarda la información para la gráfica.
            string informationLog = $"{ration} raciones de {food}.";
            _petCareLogManager.AddActionLog(ActionType.Food, new ActionLog(DateTime.Now, informationLog));

            _ = ActivateCDFood(timeCDActions);
            _ = ActivateFoodEffect(timeEffectActions);
        }

        private async Task ActivateCDFood(float time)
        {
            isFoodActionInCD = true;
            foodCooldownEndTime = DateTime.Now.AddSeconds(time);
            _petCareRepository.SaveFoodCooldownEndTime(foodCooldownEndTime);
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Food);
            await Task.Delay((int)(time * 1000));
            DeactivateFoodActionCD();
        }

        public void DeactivateFoodActionCD()
        {
            isFoodActionInCD = false;
            foodCooldownEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveFoodCooldownEndTime(foodCooldownEndTime);
            GameEvents_PetCare.OnFinishTimerCD(ActionType.Food);
        }

        private async Task ActivateFoodEffect(float time)
        {
            isFoodEffectActive = true;
            await Task.Delay((int)(time * 1000));
            DeactivateFoodEffect();
        }

        public void DeactivateFoodEffect()
        {
            foodEffectsEndTime = DateTime.Now.AddSeconds(-1);
            _petCareRepository.SaveFoodEffectsEndTime(foodEffectsEndTime);
            isFoodEffectActive = false;
        }

        public async Task<string> GetInformationFromFoodName(string foodName)
        {
            string response = "";
            string botResponse = await _chatBot.Ask(foodName);

            if (!string.IsNullOrEmpty(botResponse))
            {
                response = botResponse;
            }
            else
            {
                response = "Lo siento, ahora mismo no puedo pensar en una respuesta.";
            }

            return response;
        }

        public float ExtractRationsFromText(string text)
        {
            float rations = 0f;
            string numberString = "";
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    numberString += c;
            }
            if (!string.IsNullOrEmpty(numberString))
            {
                float.TryParse(numberString, out rations);
                rations /= 10;
            }

            return rations;
        }

        public void ScheduleNextBTCall()
        {
            SetNextIterationStartTime(nextIterationStartTime.AddSeconds(updateIntervalBTree));
        }

        public void SetNextIterationStartTime(DateTime newNextIterationStartTime)
        {
            nextIterationStartTime = newNextIterationStartTime;
            _petCareRepository.SaveNextIterationStartTime(newNextIterationStartTime);
        }
    }
}