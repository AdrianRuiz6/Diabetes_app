using System;
using UnityEngine;
using Master.Persistence;
using Master.Domain.GameEvents;

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

            GameEvents_Graph.OnInitialTimeModified += ModifyInitialHour;
            GameEvents_Graph.OnFinishTimeModified += ModifyFinishHour;
        }

        private void OnDestroy()
        {
            GameEvents_Graph.OnInitialTimeModified -= ModifyInitialHour;
            GameEvents_Graph.OnFinishTimeModified -= ModifyFinishHour;

            DataStorage.SaveInitialTime(initialTime);
            DataStorage.SaveFinishTime(finishTime);
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
            initialTime = DataStorage.LoadInitialTime();
            finishTime = DataStorage.LoadFinishTime();
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