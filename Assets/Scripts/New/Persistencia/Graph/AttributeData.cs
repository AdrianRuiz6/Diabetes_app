using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeData
{
    public string DateAndTime;
    public int Value;

    public AttributeData(DateTime? dateTime, int value)
    {
        this.DateAndTime = dateTime.ToString();
        this.Value = value;
    }
}
