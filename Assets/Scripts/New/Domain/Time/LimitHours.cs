using System;
using UnityEngine;
using Master.Persistence;
using Master.Domain.GameEvents;
using Master.Persistence.Settings;

namespace Master.Domain.Time
{
    public class LimitHours : MonoBehaviour
    {
        public static LimitHours Instance;

        public TimeSpan initialTime { get; private set; }
        public TimeSpan finishTime { get; private set; }

        private void Awake()
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

            GameEvents_Settings.OnInitialTimeModified += ModifyInitialHour;
            GameEvents_Settings.OnFinishTimeModified += ModifyFinishHour;
        }

        private void OnDestroy()
        {
            GameEvents_Settings.OnInitialTimeModified -= ModifyInitialHour;
            GameEvents_Settings.OnFinishTimeModified -= ModifyFinishHour;

            DataStorage_Settings.SaveInitialTime(initialTime);
            DataStorage_Settings.SaveFinishTime(finishTime);
        }

        //private void OnApplicationPause(bool pauseStatus)
        //{
        //    if (pauseStatus)
        //    {
        //        GameEventsGraph.OnInitialTimeModified -= ModifyInitialHour;
        //        GameEventsGraph.OnFinishTimeModified -= ModifyFinishHour;

        //        DataStorage.SaveInitialTime(initialTime);
        //        DataStorage.SaveFinishTime(finishTime);
        //    }
        //}

        void Start()
        {
            initialTime = DataStorage_Settings.LoadInitialTime();
            finishTime = DataStorage_Settings.LoadFinishTime();
        }

        private void ModifyInitialHour(int newHour)
        {
            initialTime = new TimeSpan(newHour, 0, 0);
        }

        private void ModifyFinishHour(int newHour)
        {
            finishTime = new TimeSpan(newHour, 59, 0);
        }

        public bool IsInRange(TimeSpan currentTime)
        {
            if (finishTime >= initialTime)
            {
                return currentTime >= initialTime && currentTime <= finishTime;
            }
            else
            {
                return currentTime >= initialTime || currentTime <= finishTime;
            }
        }
    }
}