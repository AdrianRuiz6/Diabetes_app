using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedingBar : MonoBehaviour
{
    public Image feedingFillBar;
    public Gradient feedingColorGradient;

    public float currentFeeding;
    public float maxFeeding;
    public float minFeeding;

    public float timePassiveDecision;
    private float timerPassiveDesion;

    private float passiveHitFeeding = 5;
    private int decisionFeeding;

    public Button sportButton;
    public Button foodButton;

    public float timeGenerateCoins;
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
        currentFeeding = Mathf.Clamp(currentFeeding, minFeeding, maxFeeding);
    }

    void Start()
    {
        UpdateFeeding(0);

        sportButton.onClick.AddListener(UseSport);
        foodButton.onClick.AddListener(UseFood);

        timerPassiveDesion = timePassiveDecision;

        timerGenerateCoins = timeGenerateCoins;
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
            Debug.Log("Current color: " + currentColor);//
            Debug.Log("Green: " + myGradientGreen);//
            Debug.Log("Orange: " + myGradientOrange);//
            Debug.Log("Red: " + myGradientRed);//

            if (AreColorSimilar(currentColor, myGradientGreen, 0.1f))
            {
                Debug.Log("verde");//
                totalCoinsGenerated += coinsGenerateGreen;
            }
            else if (AreColorSimilar(currentColor, myGradientOrange, 0.1f))
            {
                Debug.Log("naranja");//
                totalCoinsGenerated += coinsGenerateOrange;
            }
            else if (AreColorSimilar(currentColor, myGradientRed, 0.1f))
            {
                Debug.Log("rojo");//
                totalCoinsGenerated += coinsGenerateRed;
            }
            timerGenerateCoins = timeGenerateCoins;
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

    private void UpdateFeeding(float amount)
    {
        currentFeeding = Mathf.Clamp(currentFeeding + amount, minFeeding, maxFeeding);
        feedingFillBar.fillAmount = currentFeeding / maxFeeding;
        feedingFillBar.color = feedingColorGradient.Evaluate(feedingFillBar.fillAmount);
    }

    public void UseFood()
    {
        UpdateFeeding(+15);
    }

    public void UseSport()
    {
        UpdateFeeding(-10);
    }

}