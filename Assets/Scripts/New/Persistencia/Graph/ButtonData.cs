using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonData
{
    public string DateAndTime;
    public string Information;

    public ButtonData(DateTime? dateTime, string information)
    {
        this.DateAndTime = dateTime.ToString();
        this.Information = information;
    }
}
