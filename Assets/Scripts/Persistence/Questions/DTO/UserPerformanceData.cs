using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Persistence.Questions
{
    [System.Serializable]
    public class UserPerformanceData
    {
        public string topic;
        public List<string> performanceData;

        public UserPerformanceData(string playerName, List<string> performanceData)
        {
            this.topic = playerName;
            this.performanceData = performanceData;
        }
    }
}