using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedingBar : MonoBehaviour
{
    [Header("Feeding fill bar")]
    public Image feedingFillBar;
    public Gradient feedingColorGradient;

    [Header("Feeding values")]
    public float currentFeedingValue;
    public float maxFeeding;
    public float minFeeding;

    [Header("Passive decision")]
    public float timePassiveDecision;
    private float timerPassiveDesion;
    private float passiveHitFeeding = 5;
    private int decisionFeeding;

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
    public Button feedingBarButton;
    private CoinsManager coinsManager;

    private void OnValidate()
    {
        currentFeedingValue = Mathf.Clamp(currentFeedingValue, minFeeding, maxFeeding);
    }

    void Start()
    {
        UpdateFeeding(0);

        timerPassiveDesion = timePassiveDecision;

        timerGenerateCoins = timeToGenerateCoins;
        coinsManager = GetComponent<CoinsManager>();
        feedingBarButton.onClick.AddListener(ExtractCoins);
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
            decisionFeeding = Random.Range(0, 2);
            switch (decisionFeeding)
            {
                case 0:
                    UpdateFeeding(-passiveHitFeeding);
                    break;
                case 1:
                    UpdateFeeding(+passiveHitFeeding);
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
        if(timerGenerateCoins <= 0)
        {
            currentColor = feedingColorGradient.Evaluate(feedingFillBar.fillAmount);
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

    public void UpdateFeeding(float amount)
    {
        currentFeedingValue = Mathf.Clamp(currentFeedingValue + amount, minFeeding, maxFeeding);
        feedingFillBar.fillAmount = currentFeedingValue / maxFeeding;
        feedingFillBar.color = feedingColorGradient.Evaluate(feedingFillBar.fillAmount);
    }
}