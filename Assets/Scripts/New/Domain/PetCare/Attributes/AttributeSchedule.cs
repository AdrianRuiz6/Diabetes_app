using Master.Domain.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Master.Persistence;
using Master.Persistence.Connection;

public class AttributeSchedule : MonoBehaviour
{
    public static AttributeSchedule Instance;
    private DateTime _lastIterationTime = DateTime.Now;

    public float UpdateInterval = 300; // 5 minutos

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

        _lastIterationTime.AddSeconds(-UpdateInterval);
    }

    private void OnDestroy()
    {
        DataStorage_Connection.SaveLastIterationStartTime(_lastIterationTime);
    }

    public void UpdateTimer(float restTimeIteration, DateTime lastTimeInterval)
    {
        StopAllCoroutines();
        _lastIterationTime = lastTimeInterval;
        StartCoroutine(TimerAttributes(restTimeIteration));
    }

    private IEnumerator TimerAttributes(float timeFirstIteration)
    {
        yield return new WaitForSeconds(timeFirstIteration);
        GameEvents_PetCare.OnExecutingAttributes?.Invoke(DateTime.Now);

        while (true)
        {
            _lastIterationTime = DateTime.Now;
            yield return new WaitForSeconds(UpdateInterval);
            GameEvents_PetCare.OnExecutingAttributes?.Invoke(DateTime.Now);
        }
    }
}
