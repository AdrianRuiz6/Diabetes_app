using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance;

    public float glycemiaValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public float activityValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public float hungerValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public bool isInsulineButtonUsed { get; private set; }
    public bool isExerciseButtonUsed { get; private set; }
    public bool isFoodButtonUsed { get; private set; }

    private float timeEffectsButton;


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
    }

    void OnDestroy()
    {
        GameEventsPetCare.OnModifyGlycemia -= ModifyGlycemia;
        GameEventsPetCare.OnModifyActivity -= ModifyActivity;
        GameEventsPetCare.OnModifyHunger -= ModifyHunger;
    }

    void Start()
    {
        timeEffectsButton = 1800; // 30 minutos

        glycemiaValue = 120; // TODO: hacer clamp
        activityValue = 50; // TODO: hacer clamp
        hungerValue = 50; // TODO: hacer clamp

        isInsulineButtonUsed = false;
        isExerciseButtonUsed = false;
        isFoodButtonUsed = false;
    }

    private void ModifyGlycemia(int value)
    {
        glycemiaValue += value;
    }

    private void ModifyActivity(int value)
    {
        activityValue += value;
    }

    private void ModifyHunger(int value)
    {
        hungerValue += value;
    }

    public void ActivateInsulinButton(bool simulated = false, float time = 0)
    {
        isInsulineButtonUsed = true;
        if (!simulated)
        {
            ModifyGlycemia(-40);

            time = timeEffectsButton;
        }

        if (time > 0)
        {
            StartCoroutine(ResetInsulinButton(time));
        }
    }

    public void DeactivateInsulinButton()
    {
        isInsulineButtonUsed = false;
    }

    public void ActivateExerciseButton(bool simulated = false, float time = 0)
    {
        isExerciseButtonUsed = true;

        if (!simulated)
        {
            ModifyGlycemia(-30);
            ModifyActivity(30);
            ModifyHunger(10);

            time = timeEffectsButton;
        }

        if (time > 0)
        {
            StartCoroutine(ResetExerciseButton(time));
        }

    }

    public void DeactivateExerciseButton()
    {
        isExerciseButtonUsed = false;
    }

    public void ActivateFoodButton(bool simulated = false, float time = 0, int carbohydratesAmount = 0)
    {
        isFoodButtonUsed = true;

        if (!simulated)
        {
            if (carbohydratesAmount == 0)
                ModifyGlycemia(0);
            else if (carbohydratesAmount <= 30)
                ModifyGlycemia(40);
            else if (carbohydratesAmount <= 70)
                ModifyGlycemia(80);
            else
                ModifyGlycemia(120);
            ModifyHunger(-30);

            time = timeEffectsButton;
        }

        if(time > 0)
        {
            StartCoroutine(ResetFoodButton(time));
        }
    }

    public void DeactivateFoodButton()
    {
        isFoodButtonUsed = false;
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
}
