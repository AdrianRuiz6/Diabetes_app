using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonData : MonoBehaviour
{
    public string DateAndTime;
    public string Information;

    public ButtonData(DateTime dateTime, string information)
    {
        this.DateAndTime = dateTime.ToString();
        this.Information = information;
    }
}
