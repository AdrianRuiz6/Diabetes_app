using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    [System.Serializable]
    public class AttributeLogData
    {
        public string DateAndTime;
        public int Value;

        public AttributeLogData(DateTime? dateTime, int value)
        {
            this.DateAndTime = dateTime.ToString();
            this.Value = value;
        }
    }
}