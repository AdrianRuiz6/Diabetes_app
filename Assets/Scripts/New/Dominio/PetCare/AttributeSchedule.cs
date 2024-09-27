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

    public float UpdateInterval = 500; // TODO: Cambiar a 500: 5 minutos
    private float _currentUpdateInterval;

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

        _currentUpdateInterval = UpdateInterval;
        _lastTimeInterval.AddSeconds(-UpdateInterval);
    }

    private void OnDestroy()
    {
        TimeSpan timePassedInterval = DateTime.Now - _lastTimeInterval;
        float restTimeInterval = _currentUpdateInterval - (float)timePassedInterval.TotalSeconds;

        DataStorage.SaveRestTimeIntervalSimulator(restTimeInterval);
    }

    public void UpdateTimer(float newTime)
    {
        StopAllCoroutines();
        _currentUpdateInterval = newTime;
        StartCoroutine(TimerAttributes(newTime));
    }

    private IEnumerator TimerAttributes(float timeFirstInterval)
    {
        _lastTimeInterval = DateTime.Now;
        yield return new WaitForSeconds(timeFirstInterval);
        GameEventsPetCare.OnExecutingAttributes?.Invoke(DateTime.Now);

        while (true)
        {
            _lastTimeInterval = DateTime.Now;
            _currentUpdateInterval = UpdateInterval;
            yield return new WaitForSeconds(_currentUpdateInterval);
            GameEventsPetCare.OnExecutingAttributes?.Invoke(DateTime.Now);
        }
    }
}
