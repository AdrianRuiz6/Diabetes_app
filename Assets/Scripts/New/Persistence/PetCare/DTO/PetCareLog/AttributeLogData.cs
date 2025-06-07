using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    [System.Serializable]
    public class AttributeLogData
    {
        [SerializeField] private string _dateAndTime;
        [SerializeField] private int _value;

        public AttributeLogData(DateTime? dateTime, int value)
        {
            this._dateAndTime = dateTime.ToString();
            this._value = value;
        }

        public DateTime? GetDateAndTime()
        {
            return DateTime.Parse(_dateAndTime);
        }

        public int GetValue()
        {
            return _value;
        }
    }
}