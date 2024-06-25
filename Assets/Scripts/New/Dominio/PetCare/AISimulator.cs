using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimulator : MonoBehaviour
{
    private int _iterationsTotal;

    private int _iterationsEffectsInsulin;
    private bool _isInsulinActivated;
    private int _iterationsEffectsExercise;
    private bool _isExerciseActivated;
    private int _iterationsEffectsFood;
    private bool _isFoodActivated;

    private int _timeIntervalAI;
    private float _initialTimerSeconds;

    void Start()
    {
        _timeIntervalAI = 300; // 5 minutos.

        Simulate();
    }

    private void Simulate()
    {
        CalculateIterationsButtons();
        SimulateIntervals();
        AttributeSchedule.Instance.UpdateTimer(_initialTimerSeconds);
    }

    private void CalculateIterationsButtons()
    {
        DateTime? currentTime = DateTime.Now;
        TimeSpan? timePassed = null;
        float secondsPassed;
        float restTimeEffects;
        DateTime? lastTimeDisconnected = DataStorage.LoadDisconnectionDate();
        float timeLeftPreviousInterval = DataStorage.LoadTimeLeftIntervalIA();

        _isInsulinActivated = AttributeManager.Instance.isInsulinEffectActive;
        _isExerciseActivated = AttributeManager.Instance.isExerciseEffectActive;
        _isFoodActivated = AttributeManager.Instance.isFoodEffectActive;

        timePassed = currentTime - lastTimeDisconnected;
        secondsPassed = (float)timePassed.Value.TotalSeconds + timeLeftPreviousInterval;
        _iterationsTotal = (int)secondsPassed / _timeIntervalAI;
        _initialTimerSeconds = (float)secondsPassed % _timeIntervalAI;

        timePassed = AttributeManager.Instance.lastTimeInsulinUsed - lastTimeDisconnected;
        secondsPassed = (float)timePassed.Value.TotalSeconds;
        restTimeEffects = AttributeManager.Instance.timeEffectButtons - secondsPassed;
        if(restTimeEffects > 0)
        {
            AttributeManager.Instance.isInsulinEffectActive = true;
            _iterationsEffectsInsulin = (int)restTimeEffects / _timeIntervalAI;
        }

        timePassed = AttributeManager.Instance.lastTimeExerciseUsed - lastTimeDisconnected;
        secondsPassed = (float)timePassed.Value.TotalSeconds;
        restTimeEffects = AttributeManager.Instance.timeEffectButtons - secondsPassed;
        if (restTimeEffects > 0)
        {
            AttributeManager.Instance.isExerciseEffectActive = true;
            _iterationsEffectsInsulin = (int)restTimeEffects / _timeIntervalAI;
        }

        timePassed = AttributeManager.Instance.lastTimeFoodUsed - lastTimeDisconnected;
        secondsPassed = (float)timePassed.Value.TotalSeconds;
        restTimeEffects = AttributeManager.Instance.timeEffectButtons - secondsPassed;
        if (restTimeEffects > 0)
        {
            AttributeManager.Instance.isFoodEffectActive = true;
            _iterationsEffectsInsulin = (int)restTimeEffects / _timeIntervalAI;
        }
    }

    private void SimulateIntervals()
    {
        for(int iteration = 0; iteration < _iterationsTotal;  iteration++)
        {
            GameEventsPetCare.OnExecutingAttributes?.Invoke();

            _iterationsEffectsInsulin--;
            _iterationsEffectsExercise--;
            _iterationsEffectsFood--;

            if(_iterationsEffectsInsulin <= 0 && !_isInsulinActivated)
            {
                AttributeManager.Instance.isInsulinEffectActive = false;
            }
            if(_iterationsEffectsExercise <= 0 && !_isExerciseActivated)
            {
                AttributeManager.Instance.isExerciseEffectActive = false;
            }
            if(_iterationsEffectsFood <= 0 && !_isFoodActivated)
            {
                AttributeManager.Instance.isFoodEffectActive = false;
            }
        }
    }
}
