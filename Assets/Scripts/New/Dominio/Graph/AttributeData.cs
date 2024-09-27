using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeData
{
    public string DateAndTime;
    public float Value;

    public AttributeData(DateTime dateTime, float value)
    {
        this.DateAndTime = dateTime.ToString();
        this.Value = value;
    }
}
