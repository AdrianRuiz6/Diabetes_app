using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlucoseBar: MonoBehaviour
{
    [Header("Glucose fill bar")]
    public Image glucoseFillBar;
    public Gradient glucoseColorGradient;

    [Header("Glucose values")]
    public float currentGlucoseValue;
    public float maxGlucose;
    public float minGlucose;

    [Header("Passive decision")]
    public float timePassiveDecision;
    private float timerPassiveDesion;
    private float passiveHitGlucose = 5;
    private int decisionGlucose;

    [Header("Modifier buttons")]
    public Button insulineButton;
    public Button sportButton;
    public Button foodButton;

    [Header("Generative coins")]
    public float timeToGenerateCoins;
    private float timerGenerateCoins;
    public Color myGradientGreen;
    public int coinsGenerateGreen;
    public Color myGradientOrange;
    public int coinsGenerateOrange;
    public Color myGradientRed;
    public int coinsGenerateRed;
    private Color currentColor;
    public int totalCoinsGenerated;//
    public Button glucoseBarButton;
    private CoinsManager coinsManager;

    private void OnValidate()
    {
        currentGlucoseValue = Mathf.Clamp(currentGlucoseValue, minGlucose, maxGlucose);
    }

    void Start()
    {
        UpdateGlucose(0);

        insulineButton.onClick.AddListener(UseInsuline);
        sportButton.onClick.AddListener(UseSport);
        foodButton.onClick.AddListener(UseFood);

        timerPassiveDesion = timePassiveDecision;

        timerGenerateCoins = timeToGenerateCoins;
        coinsManager = GetComponent<CoinsManager>();
        glucoseBarButton.onClick.AddListener(ExtractCoins);
    }

    void Update()
    {
        WaitPassiveDecision();

        GenerateCoins();
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

    private void GenerateCoins()
    {
        timerGenerateCoins -= Time.deltaTime;
        if (timerGenerateCoins <= 0)
        {
            currentColor = glucoseColorGradient.Evaluate(glucoseFillBar.fillAmount);
            //Debug.Log("Current color: " + currentColor);
            //Debug.Log("Green: " + myGradientGreen);
            //Debug.Log("Orange: " + myGradientOrange);
            //Debug.Log("Red: " + myGradientRed);

            if (AreColorSimilar(currentColor, myGradientGreen, 0.1f))
            {
                //Debug.Log("Selected color: green");
                totalCoinsGenerated += coinsGenerateGreen;
            }
            else if (AreColorSimilar(currentColor, myGradientOrange, 0.1f))
            {
                //Debug.Log("Selected color: orange");
                totalCoinsGenerated += coinsGenerateOrange;
            }
            else if (AreColorSimilar(currentColor, myGradientRed, 0.1f))
            {
                //Debug.Log("Selected color: red");
                totalCoinsGenerated += coinsGenerateRed;
            }
            timerGenerateCoins = timeToGenerateCoins;
        }
    }

    private bool AreColorSimilar(Color color1, Color color2, float threshold)
    {
        return Vector4.Distance(color1, color2) < threshold;
    }

    public void ExtractCoins()
    {
        coinsManager.AddCoins(totalCoinsGenerated);
        totalCoinsGenerated = 0;
    }

    private void UpdateGlucose(float amount)
    {
        currentGlucoseValue = Mathf.Clamp(currentGlucoseValue + amount, minGlucose, maxGlucose);
        glucoseFillBar.fillAmount = currentGlucoseValue / maxGlucose;
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
