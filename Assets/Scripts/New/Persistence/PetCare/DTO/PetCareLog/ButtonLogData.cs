using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    [System.Serializable]
    public class ButtonLogData
    {
        public string DateAndTime;
        public string Information;

        public ButtonLogData(DateTime? dateTime, string information)
        {
            this.DateAndTime = dateTime.ToString();
            this.Information = information;
        }
    }
}