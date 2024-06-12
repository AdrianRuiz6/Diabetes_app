using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserPerformanceData
{
    public string topic;
    public List<char> performanceData;

    public UserPerformanceData(string playerName, List<char> performanceData)
    {
        this.topic = playerName;
        this.performanceData = performanceData;
    }
}
