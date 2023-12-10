using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlucoseBar: MonoBehaviour
{

    public Image glucoseFillBar;
    public Gradient glucoseColorGradient;

    public float currentGlucose;
    public float maxGlucose;
    public float minGlucose;

    public float timePassiveDecision;
    private float timerPassiveDesion;

    private float passiveHitGlucose = 5;
    private int decisionGlucose;

    public Button insulineButton;
    public Button sportButton;
    public Button foodButton;

    public Color myGradientGreen;
    public Color myGradientOrange;
    public Color myGradientRed;

    private void OnValidate()
    {
        currentGlucose = Mathf.Clamp(currentGlucose, minGlucose, maxGlucose);
    }

    void Start()
    {
        UpdateGlucose(0);

        insulineButton.onClick.AddListener(UseInsuline);
        sportButton.onClick.AddListener(UseSport);
        foodButton.onClick.AddListener(UseFood);

        timerPassiveDesion = timePassiveDecision;
    }

    void Update()
    {
        WaitPassiveDecision();
    }

    private void WaitPassiveDecision()
    {
        timerPassiveDesion -= Time.deltaTime;
        if (timerPassiveDesion <= 0)
        {
            decisionGlucose = Random.Range(0, 2);
            switch (decisionGlucose)
            {
                case 0:
                    UpdateGlucose(-passiveHitGlucose);
                    break;
                case 1:
                    UpdateGlucose(+passiveHitGlucose);
                    break;
                default:
                    break;
            }

            timerPassiveDesion += timePassiveDecision;
        }
    }

    private void UpdateGlucose(float amount)
    {
        currentGlucose = Mathf.Clamp(currentGlucose + amount, minGlucose, maxGlucose);
        glucoseFillBar.fillAmount = currentGlucose / maxGlucose;
        glucoseFillBar.color = glucoseColorGradient.Evaluate(glucoseFillBar.fillAmount);
    }

    public void UseFood()
    {
        UpdateGlucose(+15);
    }

    public void UseSport()
    {
        UpdateGlucose(-30);
    }

    public void UseInsuline()
    {
        UpdateGlucose(-15);
    }

}
