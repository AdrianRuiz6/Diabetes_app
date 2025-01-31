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

    bool previousIsInRangeTime;
    bool currentIsInRangeTime;

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
                StartCoroutine(ResetInsulinButton(timeCD));
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
                StartCoroutine(ResetExerciseButton(timeCD));
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
                StartCoroutine(ResetFoodButton(timeCD));
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
        previousIsInRangeTime = LimitHours.Instance.IsInRange(DateTime.Now.TimeOfDay);
    }

    void Update()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        currentIsInRangeTime = LimitHours.Instance.IsInRange(currentTime);

        if (previousIsInRangeTime == false && currentIsInRangeTime == true)
        {
            RestartGlycemia(DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));
            RestartActivity(DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));
            RestartHunger(DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));

            DateTime currentCheckedTime = DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours);
            while (currentCheckedTime <= DateTime.Now)
            {
                currentCheckedTime = currentCheckedTime.AddSeconds(AttributeSchedule.Instance.UpdateInterval);
            }

            AttributeSchedule.Instance.UpdateTimer((float)(currentCheckedTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalSeconds + 1, currentCheckedTime.AddSeconds(-AttributeSchedule.Instance.UpdateInterval));
        }
        previousIsInRangeTime = currentIsInRangeTime;
    }

    public void RestartGlycemia(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyGlycemia?.Invoke(initialGlycemiaValue - glycemiaValue, DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));
    }

    public void RestartActivity(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyActivity?.Invoke(initialActivityValue - activityValue, DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));
    }
    public void RestartHunger(DateTime currentTime)
    {
        GameEventsPetCare.OnModifyHunger?.Invoke(initialHungerValue - hungerValue, DateTime.Now.Date.AddHours(LimitHours.Instance.initialTime.Hours));
    }

    private void ModifyGlycemia(int value, DateTime? currentDateTime = null)
    {
        mutex.WaitOne();
        try
        {
            glycemiaValue = Mathf.Clamp(glycemiaValue + value, minGlycemiaValue, maxGlycemiaValue);
            if (currentDateTime != null)
            {
                if(currentDateTime.Value.TimeOfDay == LimitHours.Instance.initialTime)
                {
                    DataStorage.SaveGlycemiaGraph(DateTime.Now, glycemiaValue);
                }
                else
                {
                    DataStorage.SaveGlycemiaGraph(currentDateTime, glycemiaValue);
                }
                
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Glycemia);

                // Distribución de las recompensas
                if (currentDateTime.Value.TimeOfDay != LimitHours.Instance.initialTime)
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

    private void ModifyActivity(int value, DateTime? currentDateTime = null)
    {
        mutex.WaitOne();
        try
        {
            activityValue = Mathf.Clamp(activityValue + value, minActivityValue, maxActivityValue);
            if (currentDateTime != null)
            {
                if (currentDateTime.Value.TimeOfDay == LimitHours.Instance.initialTime)
                {
                    DataStorage.SaveActivityGraph(DateTime.Now, activityValue);
                }
                else
                {
                    DataStorage.SaveActivityGraph(currentDateTime, activityValue);
                }
                
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Activity);
                // Distribución de las recompensas
                if (currentDateTime.Value.TimeOfDay != LimitHours.Instance.initialTime)
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

    private void ModifyHunger(int value, DateTime? currentDateTime = null)
    {
        mutex.WaitOne();
        try
        {
            hungerValue = Mathf.Clamp(hungerValue + value, minHungerValue, maxHungerValue);
            if (currentDateTime != null)
            {
                if (currentDateTime.Value.TimeOfDay == LimitHours.Instance.initialTime)
                {
                    DataStorage.SaveHungerGraph(DateTime.Now, hungerValue);
                }
                else
                {
                    DataStorage.SaveHungerGraph(currentDateTime, hungerValue);
                }
                
                GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Hunger);
                // Distribución de las recompensas
                if (currentDateTime.Value.TimeOfDay != LimitHours.Instance.initialTime)
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
        GameEventsPetCare.OnModifyGlycemia?.Invoke(affectedGlycemia, DateTime.Now);

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

        StartCoroutine(ResetInsulinButton(timeButtonsCD));
        StartCoroutine(ActivateInsulinEffect(timeEffectButtons));
    }

    public void DeactivateInsulinButton()
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
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-30, DateTime.Now);
                GameEventsPetCare.OnModifyActivity?.Invoke(30, DateTime.Now);
                GameEventsPetCare.OnModifyHunger?.Invoke(10, DateTime.Now);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
            case "Intensidad media":
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-75, DateTime.Now);
                GameEventsPetCare.OnModifyActivity?.Invoke(50, DateTime.Now);
                GameEventsPetCare.OnModifyHunger?.Invoke(20, DateTime.Now);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
            case "Intensidad alta":
                GameEventsPetCare.OnModifyGlycemia?.Invoke(-110, DateTime.Now);
                GameEventsPetCare.OnModifyActivity?.Invoke(70, DateTime.Now);
                GameEventsPetCare.OnModifyHunger?.Invoke(30, DateTime.Now);
                Debug.Log($"EXERCISE BUTTON -Level of intensity-: {intensity}");
                break;
        }

        // Se guarda la información para la gráfica.
        string informationGraph = $"{intensity}.";
        DataStorage.SaveExerciseGraph(DateTime.Now, informationGraph);
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        StartCoroutine(ResetExerciseButton(timeButtonsCD));
        StartCoroutine(ActivateExerciseEffect(timeEffectButtons));
    }

    public void DeactivateExerciseButton()
    {
        isExerciseButtonInCD = false;
    }

    public void ActivateFoodButton(float ration, string food)
    {
        isFoodButtonInCD = true;
        lastTimeFoodUsed = DateTime.Now;

        float affectedGlycemia = ration * 34;
        Debug.Log($"FOOD BUTTON -Affected glycemia-: {affectedGlycemia}");
        GameEventsPetCare.OnModifyGlycemia?.Invoke((int)affectedGlycemia, DateTime.Now);
        GameEventsPetCare.OnModifyHunger?.Invoke(-100, DateTime.Now);

        // Se guarda la información para la gráfica.
        string informationGraph = $"{ration} raciones de {food}.";
        DataStorage.SaveFoodGraph(DateTime.Now, informationGraph);
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        StartCoroutine(ResetFoodButton(timeButtonsCD));
        StartCoroutine(ActivateFoodEffect(timeEffectButtons));
    }

    public void DeactivateFoodButton()
    {
        isFoodButtonInCD = false;
    }

    private IEnumerator ResetInsulinButton(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Insulin", time);
        yield return new WaitForSeconds(time);
        DeactivateInsulinButton();
    }

    private IEnumerator ResetExerciseButton(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Exercise", time);
        yield return new WaitForSeconds(time);
        DeactivateExerciseButton();
    }

    private IEnumerator ResetFoodButton(float time)
    {
        GameEventsPetCare.OnStartTimerCD?.Invoke("Food", time);
        yield return new WaitForSeconds(time);
        DeactivateFoodButton();
    }

    private IEnumerator ActivateInsulinEffect(float time)
    {
        isInsulinEffectActive = true;
        yield return new WaitForSeconds(time);
        isInsulinEffectActive = false;
    }

    private IEnumerator ActivateExerciseEffect(float time)
    {
        isExerciseEffectActive = true;
        yield return new WaitForSeconds(time);
        isExerciseEffectActive = false;
    }

    private IEnumerator ActivateFoodEffect(float time)
    {
        isFoodEffectActive = true;
        yield return new WaitForSeconds(time);
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