using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityBar : MonoBehaviour
{
    [Header("Activity fill bar")]
    public Image activityFillBar;
    public Gradient activityColorGradient;

    [Header("Activity values")]
    public float currentActivityValue;
    public float maxActivity;
    public float minActivity;

    [Header("Passive decision")]
    public float timePassiveDecision;
    private float timerPassiveDesion;
    private float passiveHitActivity = 5;
    private int decisionActivity;

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
    public Button activityBarButton;
    private CoinsManager coinsManager;

    private void OnValidate()
    {
        currentActivityValue = Mathf.Clamp(currentActivityValue, minActivity, maxActivity);
    }

    void Start()
    {
        UpdateActivity(0);

        timerPassiveDesion = timePassiveDecision;

        timerGenerateCoins = timeToGenerateCoins;
        coinsManager = GetComponent<CoinsManager>();
        activityBarButton.onClick.AddListener(ExtractCoins);
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
            decisionActivity = Random.Range(0, 2);
            switch (decisionActivity)
            {
                case 0:
                    UpdateActivity(-passiveHitActivity);
                    break;
                case 1:
                    UpdateActivity(+passiveHitActivity);
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
            currentColor = activityColorGradient.Evaluate(activityFillBar.fillAmount);
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

    public void UpdateActivity(float amount)
    {
        currentActivityValue = Mathf.Clamp(currentActivityValue + amount, minActivity, maxActivity);
        activityFillBar.fillAmount = currentActivityValue / maxActivity;
        activityFillBar.color = activityColorGradient.Evaluate(activityFillBar.fillAmount);
    }
}
