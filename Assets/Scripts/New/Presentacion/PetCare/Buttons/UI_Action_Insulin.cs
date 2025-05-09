using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Action_Insulin : UI_Actions_PetCare
{
    public override void UpdatedValueSlider(float value)
    {
        ValueTMP.text = value.ToString();
    }

    public override void SendInformation()
    {
        AttributeManager.Instance.ActivateInsulinAction(int.Parse(ValueTMP.text));

        base.SendInformation();
    }
}
