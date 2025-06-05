using Master.Domain.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Master.Domain.Shop;
using Master.Presentation.PetCare.Log;
using Master.Presentation.PetCare;
using Master.Persistence;
using Master.Domain.Score;
using Master.Persistence.PetCare;

namespace Master.Domain.PetCare
{
    public class AttributeManager : MonoBehaviour
    {
        public static AttributeManager Instance;

        private Mutex mutex = new Mutex();

        public List<AttributeState> GlycemiaRangeStates;
        public List<AttributeState> ActivityRangeStates;
        public List<AttributeState> HungerRangeStates;

        public int minGlycemiaValue { get; private set; }
        public int minActivityValue { get; private set; }
        public int minHungerValue { get; private set; }
        public int maxGlycemiaValue { get; private set; }
        public int maxActivityValue { get; private set; }
        public int maxHungerValue { get; private set; }
        public int initialGlycemiaValue { get; private set; }
        public int initialActivityValue { get; private set; }
        public int initialHungerValue { get; private set; }
        public int glycemiaValue { get; private set; }
        public int activityValue { get; private set; }
        public int hungerValue { get; private set; }

        [HideInInspector] public DateTime? lastTimeInsulinUsed { get; private set; }
        [HideInInspector] public DateTime? lastTimeExerciseUsed { get; private set; }
        [HideInInspector] public DateTime? lastTimeFoodUsed { get; private set; }

        [HideInInspector] public bool isInsulinActionInCD { get; private set; }
        [HideInInspector] public bool isExerciseActionInCD { get; private set; }
        [HideInInspector] public bool isFoodActionInCD { get; private set; }

        [HideInInspector] public bool isInsulinEffectActive { get; set; }
        [HideInInspector] public bool isExerciseEffectActive { get; set; }
        [HideInInspector] public bool isFoodEffectActive { get; set; }

        [HideInInspector] public float timeCDActions { get; private set; }
        [HideInInspector] public float timeEffectActions { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            GameEvents_PetCare.OnModifyGlycemia += ModifyGlycemia;
            GameEvents_PetCare.OnModifyActivity += ModifyActivity;
            GameEvents_PetCare.OnModifyHunger += ModifyHunger;

            minGlycemiaValue = 20;
            minActivityValue = 0;
            minHungerValue = 0;
            maxGlycemiaValue = 250;
            maxActivityValue = 100;
            maxHungerValue = 100;
            initialGlycemiaValue = 115;
            initialActivityValue = 50;
            initialHungerValue = 50;
        }

        void OnDestroy()
        {
            GameEvents_PetCare.OnModifyGlycemia -= ModifyGlycemia;
            GameEvents_PetCare.OnModifyActivity -= ModifyActivity;
            GameEvents_PetCare.OnModifyHunger -= ModifyHunger;
        }

        void Start()
        {
            timeCDActions = 15; // TODO: 60 minutos 3600
            timeEffectActions = 10; // TODO: 30 minutos 1800

            glycemiaValue = Persistence.PetCare.DataStorage_PetCare.LoadGlycemia();
            activityValue = Persistence.PetCare.DataStorage_PetCare.LoadActivity();
            hungerValue = Persistence.PetCare.DataStorage_PetCare.LoadHunger();

            lastTimeInsulinUsed = Persistence.PetCare.DataStorage_PetCare.LoadLastTimeInsulinUsed();
            lastTimeExerciseUsed = Persistence.PetCare.DataStorage_PetCare.LoadLastTimeExerciseUsed();
            lastTimeFoodUsed = Persistence.PetCare.DataStorage_PetCare.LoadLastTimeFoodUsed();

            DateTime? currentTime = DateTime.Now;
            TimeSpan? timePassed = null;

            if (lastTimeInsulinUsed != null)
            {
                timePassed = currentTime - lastTimeInsulinUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isInsulinActionInCD = true;
                    float timeCD = timeCDActions - (float)timePassed.Value.Seconds;
                    StartCoroutine(ActivateCDInsulin(timeCD));
                }
                else
                {
                    isInsulinActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    StartCoroutine(ActivateInsulinEffect(timeEffectActions - (float)timePassed.Value.Seconds));
                }
            }

            if (lastTimeExerciseUsed != null)
            {
                timePassed = currentTime - lastTimeExerciseUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isExerciseActionInCD = true;
                    float timeCD = timeCDActions - (float)timePassed.Value.Seconds;
                    StartCoroutine(ActivateCDExercise(timeCD));
                }
                else
                {
                    isExerciseActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    StartCoroutine(ActivateExerciseEffect(timeEffectActions - (float)timePassed.Value.Seconds));
                }
            }

            if (lastTimeFoodUsed != null)
            {
                timePassed = currentTime - lastTimeFoodUsed;
                if ((float)timePassed.Value.TotalSeconds < timeCDActions)
                {
                    isFoodActionInCD = true;
                    float timeCD = timeCDActions - (float)timePassed.Value.Seconds;
                    StartCoroutine(ActivateCDFood(timeCD));
                }
                else
                {
                    isFoodActionInCD = false;
                }

                if ((float)timePassed.Value.TotalSeconds < timeEffectActions)
                {
                    StartCoroutine(ActivateFoodEffect(timeEffectActions - (float)timePassed.Value.Seconds));
                }
            }

            AISimulator.Instance.Simulate();
        }

        public void RestartGlycemia(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(initialGlycemiaValue - glycemiaValue, currentTime, true);
        }

        public void RestartActivity(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyActivity?.Invoke(initialActivityValue - activityValue, currentTime, true);
        }
        public void RestartHunger(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(initialHungerValue - hungerValue, currentTime, true);
        }

        private void ModifyGlycemia(int value, DateTime? currentDateTime = null, bool isRestarting = false)
        {
            mutex.WaitOne();
            try
            {
                glycemiaValue = Mathf.Clamp(glycemiaValue + value, minGlycemiaValue, maxGlycemiaValue);
                if (currentDateTime != null)
                {
                    Persistence.PetCare.DataStorage_PetCare.SaveGlycemiaLog(currentDateTime, glycemiaValue);
                    Persistence.PetCare.DataStorage_PetCare.SaveGlycemia(glycemiaValue);
                    GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Glycemia);

                    // Distribución de las recompensas
                    if (!isRestarting)
                    {
                        foreach (AttributeState state in GlycemiaRangeStates)
                        {
                            if (glycemiaValue >= state.MinValue && glycemiaValue <= state.MaxValue)
                            {
                                switch (state.RangeValue)
                                {
                                    case AttributeRangeValue.Good:
                                        ScoreManager.Instance.AddScore(20, currentDateTime, "control bueno de la glucemia");
                                        EconomyManager.Instance.AddStashedCoins(10);
                                        break;
                                    case AttributeRangeValue.Intermediate:
                                        ScoreManager.Instance.AddScore(10, currentDateTime, "control regular de la glucemia");
                                        EconomyManager.Instance.AddStashedCoins(5);
                                        break;
                                    case AttributeRangeValue.Bad:
                                        ScoreManager.Instance.SubstractScore(10, currentDateTime, "control malo de la glucemia");
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }

        }

        private void ModifyActivity(int value, DateTime? currentDateTime = null, bool isRestarting = false)
        {
            mutex.WaitOne();
            try
            {
                activityValue = Mathf.Clamp(activityValue + value, minActivityValue, maxActivityValue);
                if (currentDateTime != null)
                {
                    Persistence.PetCare.DataStorage_PetCare.SaveActivityLog(currentDateTime, activityValue);
                    Persistence.PetCare.DataStorage_PetCare.SaveActivity(activityValue);
                    GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Activity);

                    // Distribución de las recompensas
                    if (!isRestarting)
                    {
                        foreach (AttributeState state in ActivityRangeStates)
                        {
                            if (activityValue >= state.MinValue && activityValue <= state.MaxValue)
                            {
                                switch (state.RangeValue)
                                {
                                    case AttributeRangeValue.Good:
                                        ScoreManager.Instance.AddScore(20, currentDateTime, "control bueno de la actividad");
                                        EconomyManager.Instance.AddStashedCoins(10);
                                        break;
                                    case AttributeRangeValue.Intermediate:
                                        ScoreManager.Instance.AddScore(10, currentDateTime, "control regular de la actividad");
                                        EconomyManager.Instance.AddStashedCoins(5);
                                        break;
                                    case AttributeRangeValue.Bad:
                                        ScoreManager.Instance.SubstractScore(10, currentDateTime, "control malo de la actividad");
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ModifyHunger(int value, DateTime? currentDateTime = null, bool isRestarting = false)
        {
            mutex.WaitOne();
            try
            {
                hungerValue = Mathf.Clamp(hungerValue + value, minHungerValue, maxHungerValue);
                if (currentDateTime != null)
                {
                    Persistence.PetCare.DataStorage_PetCare.SaveHungerLog(currentDateTime, hungerValue);
                    Persistence.PetCare.DataStorage_PetCare.SaveHunger(hungerValue);
                    GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Hunger);

                    // Distribución de las recompensas
                    if (isRestarting)
                    {
                        foreach (AttributeState state in HungerRangeStates)
                        {
                            if (hungerValue >= state.MinValue && hungerValue <= state.MaxValue)
                            {
                                switch (state.RangeValue)
                                {
                                    case AttributeRangeValue.Good:
                                        ScoreManager.Instance.AddScore(20, currentDateTime, "control bueno del hambre");
                                        EconomyManager.Instance.AddStashedCoins(10);
                                        break;
                                    case AttributeRangeValue.Intermediate:
                                        ScoreManager.Instance.AddScore(10, currentDateTime, "control regular del hambre");
                                        EconomyManager.Instance.AddStashedCoins(5);
                                        break;
                                    case AttributeRangeValue.Bad:
                                        ScoreManager.Instance.SubstractScore(10, currentDateTime, "control malo del hambre");
                                        break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void ActivateInsulinAction(int value)
        {
            isInsulinActionInCD = true;
            lastTimeInsulinUsed = DateTime.Now;

            int affectedGlycemia = value * -85;
            Debug.Log($"INSULIN BUTTON -Affected glycemia-: {affectedGlycemia}");
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(affectedGlycemia, DateTime.Now, false);

            // Se guarda la información para la gráfica.
            string informationGraph = $"";
            if (affectedGlycemia == 1)
            {
                informationGraph = $"{value} unidadad.";
            }
            else
            {
                informationGraph = $"{value} unidades.";
            }

            Persistence.PetCare.DataStorage_PetCare.SaveInsulinLog(DateTime.Now, informationGraph);
            Persistence.PetCare.DataStorage_PetCare.SaveLastTimeInsulinUsed(lastTimeInsulinUsed);
            GameEvents_PetCareLog.OnUpdatedActionsLog?.Invoke();

            StartCoroutine(ActivateCDInsulin(timeCDActions));
            StartCoroutine(ActivateInsulinEffect(timeEffectActions));
        }

        public void DeactivateInsulinActionCD()
        {
            isInsulinActionInCD = false;
        }

        public void ActivateExerciseAction(string intensity)
        {
            isExerciseActionInCD = true;
            lastTimeExerciseUsed = DateTime.Now;

            switch (intensity)
            {
                case "Intensidad baja":
                    GameEvents_PetCare.OnModifyGlycemia?.Invoke(-30, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyActivity?.Invoke(30, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyHunger?.Invoke(10, DateTime.Now, false);
                    Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                    break;
                case "Intensidad media":
                    GameEvents_PetCare.OnModifyGlycemia?.Invoke(-75, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyActivity?.Invoke(50, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyHunger?.Invoke(20, DateTime.Now, false);
                    Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                    break;
                case "Intensidad alta":
                    GameEvents_PetCare.OnModifyGlycemia?.Invoke(-110, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyActivity?.Invoke(70, DateTime.Now, false);
                    GameEvents_PetCare.OnModifyHunger?.Invoke(30, DateTime.Now, false);
                    Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                    break;
            }

            // Se guarda la información para la gráfica.
            string informationGraph = $"{intensity}.";
            Persistence.PetCare.DataStorage_PetCare.SaveExerciseLog(DateTime.Now, informationGraph);
            Persistence.PetCare.DataStorage_PetCare.SaveLastTimeExerciseUsed(lastTimeExerciseUsed);
            GameEvents_PetCareLog.OnUpdatedActionsLog?.Invoke();

            StartCoroutine(ActivateCDExercise(timeCDActions));
            StartCoroutine(ActivateExerciseEffect(timeEffectActions));
        }

        public void DeactivateExerciseActionCD()
        {
            isExerciseActionInCD = false;
        }

        public void ActivateFoodAction(float ration, string food)
        {
            isFoodActionInCD = true;
            lastTimeFoodUsed = DateTime.Now;

            float affectedGlycemia = ration * 34;
            Debug.Log($"FOOD BUTTON -Affected glycemia-: {affectedGlycemia}");
            GameEvents_PetCare.OnModifyGlycemia?.Invoke((int)affectedGlycemia, DateTime.Now, false);
            GameEvents_PetCare.OnModifyHunger?.Invoke(-40, DateTime.Now, false);

            // Se guarda la información para la gráfica.
            string informationGraph = $"{ration} raciones de {food}.";
            Persistence.PetCare.DataStorage_PetCare.SaveFoodLog(DateTime.Now, informationGraph);
            Persistence.PetCare.DataStorage_PetCare.SaveLastTimeFoodUsed(lastTimeFoodUsed);
            GameEvents_PetCareLog.OnUpdatedActionsLog?.Invoke();

            StartCoroutine(ActivateCDFood(timeCDActions));
            StartCoroutine(ActivateFoodEffect(timeEffectActions));
        }

        public void DeactivateFoodActionCD()
        {
            isFoodActionInCD = false;
        }

        private IEnumerator ActivateCDInsulin(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke("Insulin", time);
            yield return new WaitForSeconds(time);
            DeactivateInsulinActionCD();
        }

        private IEnumerator ActivateCDExercise(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke("Exercise", time);
            yield return new WaitForSeconds(time);
            DeactivateExerciseActionCD();
        }

        private IEnumerator ActivateCDFood(float time)
        {
            GameEvents_PetCare.OnStartTimerCD?.Invoke("Food", time);
            yield return new WaitForSeconds(time);
            DeactivateFoodActionCD();
        }

        private IEnumerator ActivateInsulinEffect(float time)
        {
            isInsulinEffectActive = true;
            yield return new WaitForSeconds(time);
            DeactivateInsulinEffect();
        }

        public void DeactivateInsulinEffect()
        {
            isInsulinEffectActive = false;
        }

        private IEnumerator ActivateExerciseEffect(float time)
        {
            isExerciseEffectActive = true;
            yield return new WaitForSeconds(time);
            DeactivateExerciseEffect();
        }

        public void DeactivateExerciseEffect()
        {
            isExerciseEffectActive = false;
        }

        private IEnumerator ActivateFoodEffect(float time)
        {
            isFoodEffectActive = true;
            yield return new WaitForSeconds(time);
            DeactivateFoodEffect();
        }

        public void DeactivateFoodEffect()
        {
            isFoodEffectActive = false;
        }

        public bool IsGlycemiaInRange(int value, string stateRequested)
        {
            foreach (AttributeState currentState in GlycemiaRangeStates)
            {
                if (currentState.Name.ToLower() != stateRequested.ToLower())
                    continue;

                if (value >= currentState.MinValue && value <= currentState.MaxValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsActivityInRange(int value, string stateRequested)
        {
            foreach (AttributeState currentState in ActivityRangeStates)
            {
                if (currentState.Name.ToLower() != stateRequested.ToLower())
                    continue;

                if (value >= currentState.MinValue && value <= currentState.MaxValue)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsHungerInRange(int value, string stateRequested)
        {
            foreach (AttributeState currentState in HungerRangeStates)
            {
                if (currentState.Name.ToLower() != stateRequested.ToLower())
                    continue;

                if (value >= currentState.MinValue && value <= currentState.MaxValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}