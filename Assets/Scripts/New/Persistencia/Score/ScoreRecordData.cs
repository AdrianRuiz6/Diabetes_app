using System;
using UnityEngine;

namespace Master.Persistence.Score
{
    [System.Serializable]
    public class ScoreRecordData
    {
        [SerializeField] private string time;
        [SerializeField] private string info;
        [System.NonSerialized] public GameObject element;

        public ScoreRecordData(DateTime time, string info, GameObject element)
        {
            this.time = time.ToString();
            this.info = info;
            this.element = element;
        }

        public DateTime GetTime()
        {
            return DateTime.Parse(time);
        }

        public string GetInfo()
        {
            return info;
        }

        public GameObject GetElement()
        {
            return element;
        }
    }
}