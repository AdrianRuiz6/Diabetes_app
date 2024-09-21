using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Insulin : ButtonsPetCare
{
    public override void UpdatedValueSlider(float value)
    {
        ValueTMP.text = value.ToString();
    }

    public override void SendInformation()
    {
        AttributeManager.Instance.ActivateInsulinButton(int.Parse(ValueTMP.text));

        base.SendInformation();
    }
}
