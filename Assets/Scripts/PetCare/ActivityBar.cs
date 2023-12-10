using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityBar : MonoBehaviour
{
    public Image activityFillBar;
    public Gradient activityColorGradient;

    public float currentActivity;
    public float maxActivity;
    public float minActivity;

    public float timePassiveDecision;
    private float timerPassiveDesion;

    private float passiveHitActivity = 5;
    private int decisionActivity;

    public Button sportButton;
    public Button foodButton;

    public Color myGradientGreen;
    public Color myGradientOrange;
    public Color myGradientRed;

    private void OnValidate()
    {
        currentActivity = Mathf.Clamp(currentActivity, minActivity, maxActivity);
    }

    void Start()
    {
        UpdateActivity(0);

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
    private void UpdateActivity(float amount)
    {
        currentActivity = Mathf.Clamp(currentActivity + amount, minActivity, maxActivity);
        activityFillBar.fillAmount = currentActivity / maxActivity;
        activityFillBar.color = activityColorGradient.Evaluate(activityFillBar.fillAmount);
    }

    public void UseSport()
    {
        UpdateActivity(+15);
    }

    public void UseFood()
    {
        UpdateActivity(-15);
    }
}
