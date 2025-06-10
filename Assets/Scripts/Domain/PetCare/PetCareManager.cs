using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using Master.Persistence.PetCare;
using Master.Domain.GameEvents;
using Master.Domain.Shop;
using Master.Domain.Score;
using Master.Domain.PetCare.Log;
using Master.Auxiliar;
using Master.Domain.Connection;

namespace Master.Domain.PetCare
{
    public class PetCareManager : IPetCareManager
    {
        private Mutex _mutex = new Mutex();
        private IChatBot _chatBot;
        private IPetCareRepository _petCareRepository;
        private IPetCareLogManager _petCareLogManager;
        private IScoreManager _scoreManager;
        private IScoreLogManager _scoreLogManager;
        private IEconomyManager _economyManager;

        // Simulation
        public float updateIntervalBTree { get; private set; }
        public DateTime lastIterationBTreeStartTime { get; private set; }

        // Attributes Ranges
        public List<AttributeState> glycemiaRangeStates { get; private set; }
        public List<AttributeState> activityRangeStates { get; private set; }
        public List<AttributeState> hungerRangeStates { get; private set; }

        // Minimum and maximum attributes values
        public int minGlycemiaValue { get; private set; }
        public int minActivityValue { get; private set; }
        public int minHungerValue { get; private set; }
        public int maxGlycemiaValue { get; private set; }
        public int maxActivityValue { get; private set; }
        public int maxHungerValue { get; private set; }
        public int initialGlycemiaValue { get; private set; }
        public int initialActivityValue { get; private set; }
        public int initialHungerValue { get; private set; }

        // Attributes values
        public int glycemiaValue { get; private set; }
        public int activityValue { get; private set; }
        public int hungerValue { get; private set; }

        // Last time actions used
        public DateTime? lastTimeInsulinUsed { get; private set; }
        public DateTime? lastTimeExerciseUsed { get; private set; }
        public DateTime? lastTimeFoodUsed { get; private set; }

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
        public float currentTimeCDInsulin { get; private set; }
        public float currentTimeCDFood { get; private set; }
        public float currentTimeCDExercise { get; private set; }
        public float timeEffectActions { get; private set; }

        public PetCareManager(IChatBot chatBot, IPetCareRepository petCareRepository, IPetCareLogManager petCareLogManager, IScoreManager scoreManager, IScoreLogManager scoreLogManager, IEconomyManager economyManager, IConnectionManager connectionManager)
        {
            _petCareRepository = petCareRepository;
            _chatBot = chatBot;
            _petCareLogManager = petCareLogManager;
            _scoreManager = scoreManager;
            _scoreLogManager = scoreLogManager;
            _economyManager = economyManager;

            updateIntervalBTree = 300; // 5 minutos

            minGlycemiaValue = 20;
            minActivityValue = 0;
            minHungerValue = 0;
            maxGlycemiaValue = 250;
            maxActivityValue = 100;
            maxHungerValue = 100;
            initialGlycemiaValue = 120;
            initialActivityValue = 50;
            initialHungerValue = 50;

            timeCDActions = 15; // TODO: 60 minutos 3600
            timeEffectActions = 10; // TODO: 30 minutos 1800

            glycemiaValue = _petCareRepository.LoadGlycemia();
            activityValue = _petCareRepository.LoadActivity();
            hungerValue = _petCareRepository.LoadHunger();

            lastTimeInsulinUsed = _petCareRepository.LoadLastTimeInsulinUsed();
            lastTimeExerciseUsed = _petCareRepository.LoadLastTimeExerciseUsed();
            lastTimeFoodUsed = _petCareRepository.LoadLastTimeFoodUsed();

            InitializeActionsEffects();
            InitializeAttributesRangeStates();
        }

        public void ExecuteAttributesBTree()
        {
            GameEvents_PetCare.OnExecuteAttributesBTree?.Invoke(DateTime.Now);
        }

        private void InitializeAttributesRangeStates()
        {
            glycemiaRangeStates = new List<AttributeState>();
            glycemiaRangeStates.Add(new AttributeState(211, 250, AttributeRangeValue.BadHigh));     // 211 - 250
            glycemiaRangeStates.Add(new AttributeState(151, 210, AttributeRangeValue.IntermediateHigh));   // 151 - 210
            glycemiaRangeStates.Add(new AttributeState(90, 150, AttributeRangeValue.Good));                // 90 - 150
            glycemiaRangeStates.Add(new AttributeState(61, 89, AttributeRangeValue.IntermediateLow));      // 61 - 89
            glycemiaRangeStates.Add(new AttributeState(21, 60, AttributeRangeValue.BadLow));           // 21 - 60

            activityRangeStates = new List<AttributeState>();
            activityRangeStates.Add(new AttributeState(81, 100, AttributeRangeValue.BadHigh));      // 81 - 100
            activityRangeStates.Add(new AttributeState(61, 80, AttributeRangeValue.IntermediateHigh));     // 61 - 80
            activityRangeStates.Add(new AttributeState(40, 60, AttributeRangeValue.Good));                 // 40 - 60
            activityRangeStates.Add(new AttributeState(20, 39, AttributeRangeValue.IntermediateLow));      // 20 - 39
            activityRangeStates.Add(new AttributeState(0, 19, AttributeRangeValue.BadLow));            // 0 - 19

            hungerRangeStates = new List<AttributeState>();
            hungerRangeStates.Add(new AttributeState(81, 100, AttributeRangeValue.BadHigh));        // 81 - 100
            hungerRangeStates.Add(new AttributeState(61, 80, AttributeRangeValue.IntermediateHigh));       // 61 - 80
            hungerRangeStates.Add(new AttributeState(40, 60, AttributeRangeValue.Good));                   // 40 - 60
            hungerRangeStates.Add(new AttributeState(20, 39, AttributeRangeValue.IntermediateLow));        // 20 - 39
            hungerRangeStates.Add(new AttributeState(0, 19, AttributeRangeValue.BadLow));              // 0 - 19
        }

        private void InitializeActionsEffects()
        {
            DateTime? currentTime = DateTime.Now;
            TimeSpan? timePassed = null;

            if (lastTimeInsulinUsed != null)
            {
                timePassed = currentTime - lastTimeInsulinUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isInsulinActionInCD = true;
                    currentTimeCDInsulin = timeCDActions - (float)timePassed.Value.Seconds;
                    _ = ActivateCDInsulin(currentTimeCDInsulin);
                }
                else
                {
                    isInsulinActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    _ = ActivateInsulinEffect(timeEffectActions - (float)timePassed.Value.Seconds);
                }
            }

            if (lastTimeExerciseUsed != null)
            {
                timePassed = currentTime - lastTimeExerciseUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isExerciseActionInCD = true;
                    currentTimeCDExercise = timeCDActions - (float)timePassed.Value.Seconds;
                    _ = ActivateCDExercise(currentTimeCDExercise);
                }
                else
                {
                    isExerciseActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    _ = ActivateExerciseEffect(timeEffectActions - (float)timePassed.Value.Seconds);
                }
            }

            if (lastTimeFoodUsed != null)
            {
                timePassed = currentTime - lastTimeFoodUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isFoodActionInCD = true;
                    float currentTimeCDFood = timeCDActions - (float)timePassed.Value.Seconds;
                    _ = ActivateCDFood(currentTimeCDFood);
                }
                else
                {
                    isFoodActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    _ = ActivateFoodEffect(timeEffectActions - (float)timePassed.Value.Seconds);
                }
            }
        }

        public void RestartGlycemia(DateTime? currentDateTime)
        {
            glycemiaValue = UtilityFunctions.Clamp(initialGlycemiaValue, minGlycemiaValue, maxGlycemiaValue);

            _petCareRepository.SaveGlycemia(initialGlycemiaValue);
            GameEvents_PetCare.OnModifyGlycemiaUI?.Invoke(initialGlycemiaValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Glycemia, new AttributeLog(currentDateTime, initialGlycemiaValue));
        }

        public void RestartActivity(DateTime? currentDateTime)
        {
            activityValue = UtilityFunctions.Clamp(initialActivityValue, minActivityValue, maxActivityValue);

            _petCareRepository.SaveActivity(initialActivityValue);
            GameEvents_PetCare.OnModifyActivityUI?.Invoke(initialActivityValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Activity, new AttributeLog(currentDateTime, initialActivityValue));
        }
        public void RestartHunger(DateTime? currentDateTime)
        {
            hungerValue = UtilityFunctions.Clamp(initialHungerValue, minHungerValue, maxHungerValue);

            _petCareRepository.SaveHunger(initialHungerValue);
            GameEvents_PetCare.OnModifyHungerUI?.Invoke(initialHungerValue);
            _petCareLogManager.AddAttributeLog(AttributeType.Hunger, new AttributeLog(currentDateTime, initialHungerValue));
        }

        public void ModifyGlycemia(int value, DateTime ? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar glucemia
                glycemiaValue = UtilityFunctions.Clamp(glycemiaValue + value, minGlycemiaValue, maxGlycemiaValue);
                _petCareRepository.SaveGlycemia(glycemiaValue);

                // Actualizar interfaz y añadir al log de atributos
                GameEvents_PetCare.OnModifyGlycemiaUI?.Invoke(glycemiaValue);

                if (isCalledByAction == false)
                {
                    _petCareLogManager.AddAttributeLog(AttributeType.Glycemia, new AttributeLog(currentDateTime, glycemiaValue));
                    // Sumar puntos y sumar monedas
                    if (IsGlycemiaInRange(AttributeRangeValue.BadHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.IntermediateHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.Good))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.IntermediateLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsGlycemiaInRange(AttributeRangeValue.BadLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ModifyActivity(int value, DateTime ? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar actividad
                activityValue = UtilityFunctions.Clamp(activityValue + value, minActivityValue, maxActivityValue);
                _petCareRepository.SaveActivity(activityValue);

                // Actualizar interfaz y añadir al log de atributos
                GameEvents_PetCare.OnModifyActivityUI?.Invoke(activityValue);

                if (isCalledByAction == false)
                {
                    _petCareLogManager.AddAttributeLog(AttributeType.Activity, new AttributeLog(currentDateTime, activityValue));
                    // Sumar puntos y sumar monedas
                    if (IsActivityInRange(AttributeRangeValue.BadHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsActivityInRange(AttributeRangeValue.IntermediateHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsActivityInRange(AttributeRangeValue.Good))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsActivityInRange(AttributeRangeValue.IntermediateLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsActivityInRange(AttributeRangeValue.BadLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ModifyHunger(int value, DateTime ? currentDateTime = null, bool isCalledByAction = false)
        {
            _mutex.WaitOne();
            try
            {
                // Modificar y guardar hambre
                hungerValue = UtilityFunctions.Clamp(hungerValue + value, minHungerValue, maxHungerValue);
                _petCareRepository.SaveHunger(hungerValue);

                // Actualizar interfaz y añadir al log de atributos
                GameEvents_PetCare.OnModifyHungerUI?.Invoke(hungerValue);

                if (isCalledByAction == false)
                {
                    _petCareLogManager.AddAttributeLog(AttributeType.Hunger, new AttributeLog(currentDateTime, hungerValue));
                    // Sumar puntos y sumar monedas
                    if (IsHungerInRange(AttributeRangeValue.BadHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.IntermediateHigh))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.Good))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.IntermediateLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                    else if (IsHungerInRange(AttributeRangeValue.BadLow))
                    {
                        int addedScore = 20;
                        _scoreManager.AddScore(addedScore, currentDateTime);
                        _scoreLogManager.AddScoreLogElement(addedScore, currentDateTime, "control malo de la glucemia");
                        _economyManager.AddStashedCoins(10);
                    }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool IsGlycemiaInRange(AttributeRangeValue attributeStateRequested)
        {
            foreach (AttributeState currentState in glycemiaRangeStates)
            {
                if (glycemiaValue >= currentState.minValue && glycemiaValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsActivityInRange(AttributeRangeValue attributeStateRequested)
        {
            foreach (AttributeState currentState in activityRangeStates)
            {
                if (activityValue >= currentState.minValue && activityValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsHungerInRange(AttributeRangeValue attributeStateRequested)
        {
            foreach (AttributeState currentState in hungerRangeStates)
            {
                if (hungerValue >= currentState.minValue && hungerValue <= currentState.maxValue && attributeStateRequested == currentState.rangeValue)
                {
                    return true;
                }
            }

            return false;
        }

        public void ActivateInsulinAction(int value)
        {
            isInsulinActionInCD = true;
            lastTimeInsulinUsed = DateTime.Now;
            _petCareRepository.SaveLastTimeInsulinUsed(lastTimeInsulinUsed);

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
            _petCareLogManager.AddActionLog(ActionType.Insulin, new ActionLog(lastTimeInsulinUsed, informationLog));

            _ = ActivateCDInsulin(timeCDActions);
            _ = ActivateInsulinEffect(timeEffectActions);
        }

        private async Task ActivateCDInsulin(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Insulin, time);
            await Task.Delay((int)(time * 1000));
            DeactivateInsulinActionCD();
        }

        public void DeactivateInsulinActionCD()
        {
            isInsulinActionInCD = false;
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
            isInsulinEffectActive = false;
        }

        public void ActivateExerciseAction(string intensity)
        {
            isExerciseActionInCD = true;
            lastTimeExerciseUsed = DateTime.Now;
            _petCareRepository.SaveLastTimeExerciseUsed(lastTimeExerciseUsed);

            switch (intensity)
            {
                case "Intensidad baja":
                    ModifyGlycemia(-30, DateTime.Now, true);
                    ModifyActivity(30, DateTime.Now, true);
                    ModifyHunger(10, DateTime.Now, true);
                    break;
                case "Intensidad media":
                    ModifyGlycemia(-75, DateTime.Now, true);
                    ModifyActivity(50, DateTime.Now, true);
                    ModifyHunger(20, DateTime.Now, true);
                    break;
                case "Intensidad alta":
                    ModifyGlycemia(-110, DateTime.Now, true);
                    ModifyActivity(70, DateTime.Now, true);
                    ModifyHunger(30, DateTime.Now, true);
                    break;
            }

            // Se guarda la información para la gráfica.
            string informationLog = $"{intensity}.";
            _petCareLogManager.AddActionLog(ActionType.Exercise, new ActionLog(lastTimeExerciseUsed, informationLog));

            _ = ActivateCDExercise(timeCDActions);
            _ = ActivateExerciseEffect(timeEffectActions);
        }

        private async Task ActivateCDExercise(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Exercise, time);
            await Task.Delay((int)(time * 1000));
            DeactivateExerciseActionCD();
        }

        public void DeactivateExerciseActionCD()
        {
            isExerciseActionInCD = false;
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
            isExerciseEffectActive = false;
        }

        public void ActivateFoodAction(float ration, string food)
        {
            isFoodActionInCD = true;
            lastTimeFoodUsed = DateTime.Now;
            _petCareRepository.SaveLastTimeFoodUsed(lastTimeFoodUsed);

            float affectedGlycemia = ration * 34;
            ModifyGlycemia((int)affectedGlycemia, DateTime.Now, true);
            ModifyHunger(-40, DateTime.Now, true);

            // Se guarda la información para la gráfica.
            string informationLog = $"{ration} raciones de {food}.";
            _petCareLogManager.AddActionLog(ActionType.Food, new ActionLog(lastTimeFoodUsed, informationLog));

            _ = ActivateCDFood(timeCDActions);
            _ = ActivateFoodEffect(timeEffectActions);
        }

        private async Task ActivateCDFood(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke(ActionType.Food, time);
            await Task.Delay((int)(time * 1000));
            DeactivateFoodActionCD();
        }

        public void DeactivateFoodActionCD()
        {
            isFoodActionInCD = false;
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
            isFoodEffectActive = false;
        }

        public async Task<string> GetInformationFromFoodName(string foodName)
        {
            string response = "";
            string botResponse = await _chatBot.AskAsync(foodName);

            if (!string.IsNullOrEmpty(botResponse))
            {
                response = botResponse;
            }
            else
            {
                response = "Lo siento, ahora mismo no puedo pensar en una respuesta :/";
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

        public void SetLastIterationStartTime(DateTime newLastIterationTime)
        {
            lastIterationBTreeStartTime = newLastIterationTime;
            _petCareRepository.SaveLastIterationStartTime(lastIterationBTreeStartTime);
        }
    }
}