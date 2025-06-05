using Master.Domain.GameEvents;
using System;
using System.Collections;
using UnityEngine;
using Master.Persistence.Connection;

public class AttributeSchedule : MonoBehaviour
{
    public static AttributeSchedule Instance;
    private DateTime _lastIterationTime;

    public float UpdateInterval = 300; // 5 minutos

    private void SetLastIterationStartTime(DateTime newLastIterationTime)
    {
        _lastIterationTime = newLastIterationTime;
        DataStorage_Connection.SaveLastIterationStartTime(_lastIterationTime);
    }

    public void UpdateTimer(float restTimeIteration, DateTime lastTimeInterval)
    {
        StopAllCoroutines();
        SetLastIterationStartTime(lastTimeInterval);
        StartCoroutine(TimerAttributes(restTimeIteration));
    }

    private IEnumerator TimerAttributes(float timeFirstIteration)
    {
        yield return new WaitForSeconds(timeFirstIteration);
        GameEvents_PetCare.OnExecutingAttributes?.Invoke(DateTime.Now);

        while (true)
        {
            SetLastIterationStartTime(DateTime.Now);
            yield return new WaitForSeconds(UpdateInterval);
            GameEvents_PetCare.OnExecutingAttributes?.Invoke(DateTime.Now);
        }
    }
}
