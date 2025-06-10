using System;
using UnityEngine;

namespace Master.Persistence.Score
{
    [System.Serializable]
    public class ScoreLogData
    {
        [SerializeField] private string time;
        [SerializeField] private string info;

        public ScoreLogData(DateTime time, string info)
        {
            this.time = time.ToString();
            this.info = info;
        }

        public DateTime GetTime()
        {
            return DateTime.Parse(time);
        }

        public string GetInfo()
        {
            return info;
        }
    }
}