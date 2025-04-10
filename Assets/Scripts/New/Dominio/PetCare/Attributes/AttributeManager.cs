using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.UIElements;
using Master.Domain.Economy;
using UnityEngine.Analytics;

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

    [HideInInspector] public bool isInsulinButtonInCD { get; private set; }
    [HideInInspector] public bool isExerciseButtonInCD { get; private set; }
    [HideInInspector] public bool isFoodButtonInCD { get; private set; }

    [HideInInspector] public bool isInsulinEffectActive { get; set; }
    [HideInInspector] public bool isExerciseEffectActive { get; set; }
    [HideInInspector] public bool isFoodEffectActive { get; set; }

    [HideInInspector] public float timeButtonsCD { get; private set; }
    [HideInInspector] public float timeEffectButtons { get; private set; }

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

        GameEventsPetCare.OnModifyGlycemia += ModifyGlycemia;
        GameEventsPetCare.OnModifyActivity += ModifyActivity;
        GameEventsPetCare.OnModifyHunger += ModifyHunger;

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
        GameEventsPetCare.OnModifyGlycemia -= ModifyGlycemia;
        GameEventsPetCare.OnModifyActivity -= ModifyActivity;
        GameEventsPetCare.OnModifyHunger -= ModifyHunger;

        DataStorage.SaveGlycemia(glycemiaValue);
        DataStorage.SaveActivity(activityValue);
        DataStorage.SaveHunger(hungerValue);

        DataStorage.SaveLastTimeInsulinUsed(lastTimeInsulinUsed);
        DataStorage.SaveLastTimeExerciseUsed(lastTimeExerciseUsed);
        DataStorage.SaveLastTimeFoodUsed(lastTimeFoodUsed);
    }

    void Start()
    {
        timeButtonsCD = 15; // TODO: 60 minutos 3600
        timeEffectButtons = 10; // TODO: 30 minutos 1800

        glycemiaValue = DataStorage.LoadGlycemia();
        activityValue = DataStorage.LoadActivity();
        hungerValue = DataStorage.LoadHunger();

        lastTimeInsulinUsed = DataStorage.LoadLastTimeInsulinUsed();
        lastTimeExerciseUsed = DataStorage.LoadLastTimeExerciseUsed();
        lastTimeFoodUsed = DataStorage.LoadLastTimeFoodUsed();

        DateTime? currentTime = DateTime.Now;
        TimeSpan? timePassed = null;

        if (lastTimeInsulinUsed != null)
        {
            timePassed = currentTime - lastTimeInsulinUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isInsulinButtonInCD = true;
                float timeCD = timeButtonsCD - (float)timePassed.Value.Seconds;
                StartCoroutine(ActivateInsulinCD(timeCD));
            }
            else
            {
                isInsulinButtonInCD = false;
            }

            if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
            {
                StartCoroutine(ActivateInsulinEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
            }
        }

        if (lastTimeExerciseUsed != null)
        {
            timePassed = currentTime - lastTimeExerciseUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isExerciseButtonInCD = true;
                float timeCD = timeButtonsCD - (float)timePassed.Value.Seconds;
                StartCoroutine(ActivateCDExercise(timeCD));
            }
            else
            {
                isExerciseButtonInCD = false;
            }

            if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
            {
                StartCoroutine(ActivateExerciseEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
            }
        }

        if (lastTimeFoodUsed != null)
        {
            timePassed = currentTime - lastTimeFoodUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isFoodButtonInCD = true;
                float timeCD = timeButtonsCD - (float)timePassed.Value.Seconds;
                StartCoroutine(ActivateCDFood(timeCD));
            }
            else
            {
                isFoodButtonInCD = false;
            }

            if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
            {
                StartCoroutine(ActivateFoodEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
            }
        }

        AISimulator.Instance.Simulate();
    }

    public void RestartGlycemia(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyGlycemia?.Invoke(initialGlycemiaValue - glycemiaValue, currentTime, true);
    }

    public void RestartActivity(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyActivity?.Invoke(initialActivityValue - activityValue, currentTime, true);
    }
    public void RestartHunger(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyHunger?.Invoke(initialHungerValue - hungerValue, currentTime, true);
    }

    private void ModifyGlycemia(int value, DateTime? currentDateTime = null, bool isRestarting = false)
    {
        mutex.WaitOne();
        try
        {
            glycemiaValue = Mathf.Clamp(glycemiaValue + value, minGlycemiaValue, maxGlycemiaValue);
            if (currentDateTime != null)
            {
                DataStorage.SaveGlycemiaGraph(currentDateTime, glycemiaValue);
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Glycemia);

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
                DataStorage.SaveActivityGraph(currentDateTime, activityValue);
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Activity);

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
                DataStorage.SaveHungerGraph(currentDateTime, hungerValue);
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Hunger);

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

    public void ActivateInsulinButton(int value)
    {
        isInsulinButtonInCD = true;
        lastTimeInsulinUsed = DateTime.Now;

        int affectedGlycemia = value * -85;
        Debug.Log($"INSULIN BUTTON -Affected glycemia-: {affectedGlycemia}");
        GameEventsPetCare.OnModifyGlycemia?.Invoke(affectedGlycemia, DateTime.Now, false);

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

        DataStorage.SaveInsulinGraph(DateTime.Now, informationGraph);
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        StartCoroutine(ActivateInsulinCD(timeButtonsCD));
        StartCoroutine(ActivateInsulinEffect(timeEffectButtons));
    }

    public void DeactivateInsulinButtonCD()
    {
        isInsulinButtonInCD = false;
    }

    public void ActivateExerciseButton(string intensity)
    {
        isExerciseButtonInCD = true;
        lastTimeExerciseUsed = DateTime.Now;

        switch (intensity)
        {
            case "Intensidad baja":
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-30, DateTime.Now, false);
                GameEventsPetCare.OnModifyActivity?.Invoke(30, DateTime.Now, false);
                GameEventsPetCare.OnModifyHunger?.Invoke(10, DateTime.Now, false);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
            case "Intensidad media":
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-75, DateTime.Now, false);
                GameEventsPetCare.OnModifyActivity?.Invoke(50, DateTime.Now, false);
                GameEventsPetCare.OnModifyHunger?.Invoke(20, DateTime.Now, false);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
            case "Intensidad alta":
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-110, DateTime.Now, false);
                GameEventsPetCare.OnModifyActivity?.Invoke(70, DateTime.Now, false);
                GameEventsPetCare.OnModifyHunger?.Invoke(30, DateTime.Now, false);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
        }

        // Se guarda la información para la gráfica.
        string informationGraph = $"{intensity}.";
        DataStorage.SaveExerciseGraph(DateTime.Now, informationGraph);
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        StartCoroutine(ActivateCDExercise(timeButtonsCD));
        StartCoroutine(ActivateExerciseEffect(timeEffectButtons));
    }

    public void DeactivateExerciseButtonCD()
    {
        isExerciseButtonInCD = false;
    }

    public void ActivateFoodButton(float ration, string food)
    {
        isFoodButtonInCD = true;
        lastTimeFoodUsed = DateTime.Now;

        float affectedGlycemia = ration * 34;
        Debug.Log($"FOOD BUTTON -Affected glycemia-: {affectedGlycemia}");
        GameEventsPetCare.OnModifyGlycemia?.Invoke((int)affectedGlycemia, DateTime.Now, false);
        GameEventsPetCare.OnModifyHunger?.Invoke(-40, DateTime.Now, false);

        // Se guarda la información para la gráfica.
        string informationGraph = $"{ration} raciones de {food}.";
        DataStorage.SaveFoodGraph(DateTime.Now, informationGraph);
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        StartCoroutine(ActivateCDFood(timeButtonsCD));
        StartCoroutine(ActivateFoodEffect(timeEffectButtons));
    }

    public void DeactivateFoodButtonCD()
    {
        isFoodButtonInCD = false;
    }

    private IEnumerator ActivateInsulinCD(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Insulin", time);
        yield return new WaitForSeconds(time);
        DeactivateInsulinButtonCD();
    }

    private IEnumerator ActivateCDExercise(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Exercise", time);
        yield return new WaitForSeconds(time);
        DeactivateExerciseButtonCD();
    }

    private IEnumerator ActivateCDFood(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Food", time);
        yield return new WaitForSeconds(time);
        DeactivateFoodButtonCD();
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
            if (currentState.Name != stateRequested)
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
            if (currentState.Name != stateRequested)
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
            if (currentState.Name != stateRequested)
                continue;

            if (value >= currentState.MinValue && value <= currentState.MaxValue)
            {
                return true;
            }
        }

        return false;
    }
}