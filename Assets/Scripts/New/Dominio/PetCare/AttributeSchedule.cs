using Master.Domain.Events;
using Master.Domain.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSchedule : MonoBehaviour
{
    public static AttributeSchedule Instance;

    private float updateInterval;

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

    void Start()
    {
        updateInterval = 300; // 5 minutos
        StartCoroutine(TimerAttributes(updateInterval));
    }

    public void UpdateTimer(float newTime)
    {
        StopAllCoroutines();
        StartCoroutine(TimerAttributes(newTime));
    }

    private IEnumerator TimerAttributes(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            GameEventsPetCare.OnExecutingAttributes?.Invoke();
        }
    }
}
