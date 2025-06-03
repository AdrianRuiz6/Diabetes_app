using System;
using UnityEngine;
using Master.Persistence;
using Master.Persistence.Connection;

namespace Master.Domain.Time
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance;
        public DateTime lastDisconnectionDateTime;
        public DateTime currentConnectionDateTime;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            lastDisconnectionDateTime = DataStorage_Connection.LoadDisconnectionDate();
            currentConnectionDateTime = DateTime.Now;
        }

        public bool IsConnected(DateTime dateTimeToEvaluate)
        {
            if (dateTimeToEvaluate > lastDisconnectionDateTime && dateTimeToEvaluate < currentConnectionDateTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnDestroy()
        {
            DataStorage_Connection.SaveDisconnectionDate();
        }
    }
}