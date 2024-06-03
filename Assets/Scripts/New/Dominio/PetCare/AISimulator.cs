using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimulator : MonoBehaviour
{
    private DateTime lastShutDownTime; //TODO: guardar en playerPrefs
    private int countAttributes;

    void Awake()
    {
        GameEventsPetCare.OnExecutedAttribute += UpdateInterval;
    }

    private void Start()
    {
        countAttributes = 0;
        OnApplicationStart();
    }
    void OnDestroy()
    {
        GameEventsPetCare.OnExecutedAttribute += UpdateInterval;
        //TODO: guardar lastShutDownTime en playerPrefs
    }
    private void OnApplicationStart()
    {
        //TODO: cargar lastShutDownTime en playerPrefs
        SimulateTime();
    }

    private void SimulateTime()
    {
        TimeSpan timePassed = DateTime.Now - lastShutDownTime;
        int intervalsPassed = (int)(timePassed.TotalSeconds / 300);

        while (intervalsPassed != 0)
        {
            GameEventsPetCare.OnExecutingAttributes?.Invoke();
            while (countAttributes != 3)
            {
                continue;
            }
            countAttributes = 0;
        }

        UpdateTimers();
    }

    private void UpdateInterval()
    {
        countAttributes++;
    }

    private void UpdateTimers() //Actualiza los timers de los CD de los botones, de las iteraciones y de la media hora del efecto de los botones.
    {

    }
}
