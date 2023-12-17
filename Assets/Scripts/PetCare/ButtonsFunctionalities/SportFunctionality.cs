using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SportFunctionality : MonoBehaviour
{

    private Button sportButton;

    public GameObject systemsObject;
    private FeedingBar feedingBarFunctionality;
    private ActivityBar activityBarFunctionality;
    private GlucoseBar glucoseBarFunctionality;

    public float modifierFeedingBar;
    public float modifierActivityBar;
    public float modifierGlucoseBar;

    void Start()
    {
        sportButton = GetComponent<Button>();
        sportButton.onClick.AddListener(UpdateBars);

        feedingBarFunctionality = systemsObject.GetComponent<FeedingBar>();
        activityBarFunctionality = systemsObject.GetComponent<ActivityBar>();
        glucoseBarFunctionality = systemsObject.GetComponent<GlucoseBar>();
    }

    private void UpdateBars()
    {
        feedingBarFunctionality.UpdateFeeding(modifierFeedingBar);
        activityBarFunctionality.UpdateActivity(modifierActivityBar);
        glucoseBarFunctionality.UpdateGlucose(modifierGlucoseBar);
    }
}
