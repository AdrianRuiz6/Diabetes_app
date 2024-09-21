using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Button_Exercise : ButtonsPetCare
{
    public override void UpdatedValueSlider(float value)
    {
        switch (value)
        {
            case 1:
                ValueTMP.text = "Intensidad baja";
                break;
            case 2:
                ValueTMP.text = "Intensidad media";
                break;
            case 3:
                ValueTMP.text = "Intensidad alta";
                break;
        }
    }

    public override void SendInformation()
    {
        AttributeManager.Instance.ActivateExerciseButton(ValueTMP.text);

        base.SendInformation();
    }
}
