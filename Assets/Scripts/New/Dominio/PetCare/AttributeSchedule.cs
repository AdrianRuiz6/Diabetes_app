using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class AttributeSchedule : MonoBehaviour
{
    public static AttributeSchedule Instance;
    private DateTime _lastTimeInterval = DateTime.Now;

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

        _lastTimeInterval.AddSeconds(-UpdateInterval);
    }

    private void OnDestroy()
    {
         DataStorage.SaveLastIterationStartTime(_lastTimeInterval);
    }

    public void UpdateTimer(float passedTime, DateTime lastTimeInterval)
    {
        StopAllCoroutines();
        _lastTimeInterval = lastTimeInterval;
        StartCoroutine(TimerAttributes(passedTime));
    }

    private IEnumerator TimerAttributes(float timeFirstInterval)
    {
        yield return new WaitForSeconds(timeFirstInterval);
        GameEventsPetCare.OnExecutingAttributes?.Invoke(DateTime.Now);

        while (true)
        {
            _lastTimeInterval = DateTime.Now;
            yield return new WaitForSeconds(UpdateInterval);
            GameEventsPetCare.OnExecutingAttributes?.Invoke(DateTime.Now);
        }
    }
}
