using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance;

    [SerializeField] private Button _insulinButton;
    [SerializeField] private Button _exerciseButton;
    [SerializeField] private Button _foodButton;

    public float glycemiaValue { get; private set; }
    public float activityValue { get; private set; }
    public float hungerValue { get; private set; }

    [HideInInspector] public DateTime? lastTimeInsulinUsed { get; private set; }
    [HideInInspector] public DateTime? lastTimeExerciseUsed { get; private set; }
    [HideInInspector] public DateTime? lastTimeFoodUsed { get; private set; }

    [HideInInspector] public bool isInsulinButtonUsed { get; private set; }
    [HideInInspector] public bool isExerciseButtonUsed { get; private set; }
    [HideInInspector] public bool isFoodButtonUsed { get; private set; }

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
        timeButtonsCD = 3600; // 60 minutos
        timeEffectButtons = 1800; // 30 minutos

        _insulinButton.onClick.AddListener(ActivateInsulinButton);
        _exerciseButton.onClick.AddListener(ActivateExerciseButton);
        //_foodButton.onClick.AddListener(); TODO

        glycemiaValue = DataStorage.LoadGlycemia();
        GameEventsPetCare.OnModifyGlycemia?.Invoke(0);
        activityValue = DataStorage.LoadActivity();
        GameEventsPetCare.OnModifyActivity?.Invoke(0);
        hungerValue = DataStorage.LoadHunger();
        GameEventsPetCare.OnModifyHunger?.Invoke(0);

        lastTimeInsulinUsed = DataStorage.LoadLastTimeInsulinUsed();
        lastTimeExerciseUsed = DataStorage.LoadLastTimeExerciseUsed();
        lastTimeFoodUsed = DataStorage.LoadLastTimeFoodUsed();

        DateTime? currentTime = DateTime.Now;
        TimeSpan? timePassed = null;

        if(lastTimeInsulinUsed != null)
        {
            timePassed = currentTime - lastTimeInsulinUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isInsulinButtonUsed = true;
                GameEventsPetCare.OnActivateCoolDown?.Invoke("Insulin");
                StartCoroutine(ResetInsulinButton(timeButtonsCD - (float)timePassed.Value.Seconds));

                if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
                {
                    StartCoroutine(ActivateInsulinEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
                }
            }
            else
            {
                isInsulinButtonUsed = false;
                GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Insulin");
            }
        }

        if (lastTimeExerciseUsed != null)
        {
            timePassed = currentTime - lastTimeExerciseUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isExerciseButtonUsed = true;
                GameEventsPetCare.OnActivateCoolDown?.Invoke("Exercise");
                StartCoroutine(ResetExerciseButton(timeButtonsCD - (float)timePassed.Value.Seconds));

                if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
                {
                    StartCoroutine(ActivateExerciseEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
                }
            }
            else
            {
                isExerciseButtonUsed = false;
                GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Exercise");
            }
        }
        
        if (lastTimeFoodUsed != null)
        {
            timePassed = currentTime - lastTimeFoodUsed;
            if ((float)timePassed.Value.TotalSeconds < timeButtonsCD)
            {
                isFoodButtonUsed = true;
                GameEventsPetCare.OnActivateCoolDown?.Invoke("Food");
                StartCoroutine(ResetFoodButton(timeButtonsCD - (float)timePassed.Value.Seconds));

                if ((float)timePassed.Value.TotalSeconds < timeEffectButtons)
                {
                    StartCoroutine(ActivateFoodEffect(timeEffectButtons - (float)timePassed.Value.Seconds));
                }
            }
            else
            {
                isFoodButtonUsed = false;
                GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Food");
            }
        }
        
    }

    private void ModifyGlycemia(int value)
    {
        glycemiaValue = Mathf.Clamp(glycemiaValue + value, 20, 350);
    }

    private void ModifyActivity(int value)
    {
        activityValue = Mathf.Clamp(activityValue + value, 0, 100);
    }

    private void ModifyHunger(int value)
    {
        hungerValue = Mathf.Clamp(hungerValue + value, 0, 100);
    }

    public void ActivateInsulinButton()
    {
        isInsulinButtonUsed = true;
        lastTimeInsulinUsed = DateTime.Now;

        GameEventsPetCare.OnActivateCoolDown?.Invoke("Insulin");
        ModifyGlycemia(-40);

        StartCoroutine(ResetInsulinButton(timeButtonsCD));
        StartCoroutine(ActivateInsulinEffect(timeEffectButtons));
    }

    public void DeactivateInsulinButton()
    {
        isInsulinButtonUsed = false;
        GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Insulin");
    }

    public void ActivateExerciseButton()
    {
        isExerciseButtonUsed = true;
        lastTimeExerciseUsed = DateTime.Now;

        GameEventsPetCare.OnActivateCoolDown?.Invoke("Exercise");
        ModifyGlycemia(-30);
        ModifyActivity(30);
        ModifyHunger(10);

        StartCoroutine(ResetExerciseButton(timeButtonsCD));
        StartCoroutine(ActivateExerciseEffect(timeEffectButtons));
    }

    public void DeactivateExerciseButton()
    {
        isExerciseButtonUsed = false;
        GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Exercise");
    }

    public void ActivateFoodButton(int carbohydratesAmount)
    {
        isFoodButtonUsed = true;
        lastTimeFoodUsed = DateTime.Now;

        GameEventsPetCare.OnActivateCoolDown?.Invoke("Food");
        if (carbohydratesAmount == 0)
            ModifyGlycemia(0);
        else if (carbohydratesAmount <= 30)
            ModifyGlycemia(40);
        else if (carbohydratesAmount <= 70)
            ModifyGlycemia(80);
        else
            ModifyGlycemia(120);
        ModifyHunger(-30);

        StartCoroutine(ResetFoodButton(timeButtonsCD));
        StartCoroutine(ActivateFoodEffect(timeEffectButtons));
    }

    public void DeactivateFoodButton()
    {
        isFoodButtonUsed = false;
        GameEventsPetCare.OnDeactivateCoolDown?.Invoke("Food");

    }

    private IEnumerator ResetInsulinButton(float time)
    {
        yield return new WaitForSeconds(time);
        DeactivateInsulinButton();
    }

    private IEnumerator ResetExerciseButton(float time)
    {
        yield return new WaitForSeconds(time);
        DeactivateExerciseButton();
    }

    private IEnumerator ResetFoodButton(float time)
    {
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
}