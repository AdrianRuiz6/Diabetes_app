using System;
using UnityEngine;

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

        lastDisconnectionDateTime = DataStorage.LoadDisconnectionDate();
        currentConnectionDateTime = DateTime.Now;
    }

    public bool IsConnected(DateTime dateTimeToEvaluate)
    {
        if(dateTimeToEvaluate > lastDisconnectionDateTime && dateTimeToEvaluate < currentConnectionDateTime)
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
        DataStorage.SaveDisconnectionDate();
    }
}
