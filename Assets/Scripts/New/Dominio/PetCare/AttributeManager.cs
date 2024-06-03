using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance;

    public float glycemiaValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public float activityValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public float hungerValue { get; private set; } //TODO: guardar y cargar los valores en json o playerPrefs.
    public bool isInsulineButtonUsed { get; private set; } // TODO: guardar y cargar los valores en json o playerPrefs.
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

    public void ModifyGlycemia(float value)
    {
        glycemiaValue += value;
    }

    public void ModifyActivity(float value)
    {
        activityValue += value;
    }

    public void ModifyHunger(float value)
    {
        hungerValue += value;
    }

    public void ActivateInsulineButton()
    {
        ModifyGlycemia(-40);
        isInsulineButtonUsed = true;
        StartCoroutine(ResetInsulineButton());
    }

    public void ActivateExerciseButton()
    {
        ModifyGlycemia(-30);
        ModifyActivity(30);
        ModifyHunger(10);
        isExerciseButtonUsed = true;
        StartCoroutine(ResetExerciseButton());
    }

    public void ActivateFoodButton(int carbohydratesAmount)
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
        isFoodButtonUsed = true;
        StartCoroutine(ResetFoodButton());
    }

    private IEnumerator ResetInsulineButton()
    {
        yield return new WaitForSeconds(timeEffectsButton);
        isInsulineButtonUsed = false;
    }

    private IEnumerator ResetExerciseButton()
    {
        yield return new WaitForSeconds(timeEffectsButton);
        isExerciseButtonUsed = false;
    }

    private IEnumerator ResetFoodButton()
    {
        yield return new WaitForSeconds(timeEffectsButton);
        isFoodButtonUsed = false;
    }
}
