using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsulineFunctionality : MonoBehaviour
{

    private Button insulineButton;

    public GameObject systemsObject;
    private GlucoseBar glucoseBarFunctionality;

    public float modifierGlucoseBar;

    void Start()
    {
        insulineButton = GetComponent<Button>();
        insulineButton.onClick.AddListener(UpdateBars);

        glucoseBarFunctionality = systemsObject.GetComponent<GlucoseBar>();
    }

    private void UpdateBars()
    {
        glucoseBarFunctionality.UpdateGlucose(modifierGlucoseBar);
    }
}
