using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    [System.Serializable]
    public class ActionLogData
    {
        [SerializeField] private string _dateAndTime;
        [SerializeField] private string _information;

        public ActionLogData(DateTime? dateTime, string information)
        {
            this._dateAndTime = dateTime.ToString();
            this._information = information;
        }

        public DateTime? GetDateAndTime()
        {
            return DateTime.Parse(_dateAndTime);
        }

        public string GetInformation()
        {
            return _information;
        }
    }
}