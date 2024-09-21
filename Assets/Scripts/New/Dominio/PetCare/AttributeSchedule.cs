using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSchedule : MonoBehaviour
{
    public static AttributeSchedule Instance;
    private DateTime _lastTimeInterval;

    public float UpdateInterval = 60; // TODO: Cambiar a 500
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
    }

    private void OnDestroy()
    {
        TimeSpan timePassedInterval = DateTime.Now - _lastTimeInterval;
        float timeRestInterval = _currentUpdateInterval - (float)timePassedInterval.TotalSeconds;

        DataStorage.SaveTimeLeftIntervalIA(timeRestInterval);
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
        GameEventsPetCare.OnExecutingAttributes?.Invoke();

        while (true)
        {
            _lastTimeInterval = DateTime.Now;
            _currentUpdateInterval = UpdateInterval;
            yield return new WaitForSeconds(_currentUpdateInterval);
            GameEventsPetCare.OnExecutingAttributes?.Invoke();
        }
    }
}
