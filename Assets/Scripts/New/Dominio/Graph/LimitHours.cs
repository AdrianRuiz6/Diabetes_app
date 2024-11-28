using Master.Domain.Economy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        GameEventsGraph.OnInitialTimeModified += ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified += ModifyFinishHour;
    }

    private void OnDestroy()
    {
        GameEventsGraph.OnInitialTimeModified -= ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified -= ModifyFinishHour;

        DataStorage.SaveInitialTime(initialTime);
        DataStorage.SaveFinishTime(finishTime);
    }

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
        finishTime = new TimeSpan(newHour - 1, 59, 0);
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
