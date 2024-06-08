using Master.Domain.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserPerformanceData : MonoBehaviour
{
    public Dictionary<string, FixedSizeQueue<char>> userPerformance;

    public UserPerformanceData()
    {
        userPerformance = new Dictionary<string, FixedSizeQueue<char>>();
    }
    public UserPerformanceData(Dictionary<string, FixedSizeQueue<char>> newUserPerformance)
    {
        UtilityFunctions.CopyDictionaryPerformance(newUserPerformance, userPerformance);
    }
}
