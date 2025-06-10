using System;
using UnityEngine;

namespace Master.Persistence.Score
{
    public class ScoreLog
    {
        private DateTime time;
        private string info;

        public ScoreLog(DateTime time, string info)
        {
            this.time = time;
            this.info = info;
        }

        public DateTime GetTime()
        {
            return time;
        }

        public string GetInfo()
        {
            return info;
        }
    }
}