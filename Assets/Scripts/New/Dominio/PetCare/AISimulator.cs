using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimulator : MonoBehaviour
{
    public static AISimulator Instance;

    private int _iterationsTotal;

    private int _iterationsEffectsInsulin;
    private bool _isInsulinActivated;
    private int _iterationsEffectsExercise;
    private bool _isExerciseActivated;
    private int _iterationsEffectsFood;
    private bool _isFoodActivated;

    private float _timeIntervalAI;
    private float _initialTimerSeconds;

    private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

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

    public void Simulate()
    {
        _timeIntervalAI = AttributeSchedule.Instance.UpdateInterval;
        Debug.Log($"SIMULATION: Start Simulation - Time Interval: {_timeIntervalAI} seconds");

        CalculateIterationsButtons();
        SimulateIntervals();
        AttributeSchedule.Instance.UpdateTimer(_initialTimerSeconds);
    }

    private void CalculateIterationsButtons()
    {
        DateTime? currentTime = DateTime.Now;
        TimeSpan? timePassed;
        float secondsPassed;
        float restTimeEffects;
        float timeDisconnected;
        DateTime? lastTimeDisconnected = DataStorage.LoadDisconnectionDate();
        DateTime? newLastTimeDisconnected = null;
        float restTimePreviousInterval = DataStorage.LoadRestTimeIntervalSimulator();

        _isInsulinActivated = AttributeManager.Instance.isInsulinEffectActive;
        _isExerciseActivated = AttributeManager.Instance.isExerciseEffectActive;
        _isFoodActivated = AttributeManager.Instance.isFoodEffectActive;

        // Cálculo de las iteraciones e inicio del tiemer de la IA de los atributos.
        Debug.Log($"SIMULATION: Current Time: {currentTime}");
        Debug.Log($"SIMULATION: Last Disconnected Time: {lastTimeDisconnected}, Time Left from Previous Interval: {restTimePreviousInterval}");

        if (DataStorage.LoadIsFirstUsage())
        {
            _iterationsTotal = 0;
            _initialTimerSeconds = _timeIntervalAI;
        }
        else
        {
            // Verifica si lastTimeDisconnected es null
            if (lastTimeDisconnected.HasValue)
            {
                if (lastTimeDisconnected.Value.AddSeconds(restTimePreviousInterval) <= currentTime)
                {
                    DateTime dateTimeToIntroduce = DateTime.Now;

                    _iterationsTotal = 1;
                    newLastTimeDisconnected = lastTimeDisconnected.Value.AddSeconds(restTimePreviousInterval);
                    // Se guarda la fecha en la lista de fechas.
                    dateTimeToIntroduce = lastTimeDisconnected.Value.AddSeconds(restTimePreviousInterval);
                    _dateTimesQueue.Enqueue(dateTimeToIntroduce);

                    timePassed = currentTime - newLastTimeDisconnected;
                    secondsPassed = (float)timePassed?.TotalSeconds;
                    _iterationsTotal = _iterationsTotal + (int)secondsPassed / (int)_timeIntervalAI;
                    // Se guardan las fechas en la lista de fechas.
                    for (int iterations = 0; iterations < _iterationsTotal; iterations++)
                    {
                        dateTimeToIntroduce = dateTimeToIntroduce.AddSeconds(_timeIntervalAI);
                        _dateTimesQueue.Enqueue(dateTimeToIntroduce);
                    }
                    _initialTimerSeconds = secondsPassed % _timeIntervalAI;
                }
                else
                {
                    timePassed = currentTime - lastTimeDisconnected;
                    secondsPassed = restTimePreviousInterval - (float)timePassed?.TotalSeconds;
                    _iterationsTotal = 0;
                    _initialTimerSeconds = secondsPassed % _timeIntervalAI;
                }
            }
            else
            {
                _iterationsTotal = 0;
                _initialTimerSeconds = _timeIntervalAI;
            }

            Debug.Log($"SIMULATION: Total iterations: {_iterationsTotal}, Initial Timer Seconds: {_initialTimerSeconds}");
        }

        // Verificación para lastTimeInsulinUsed
        if (AttributeManager.Instance.lastTimeInsulinUsed.HasValue)
        {
            timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeInsulinUsed.Value;
            secondsPassed = (float)timePassed?.TotalSeconds;
            restTimeEffects = AttributeManager.Instance.timeEffectButtons - secondsPassed;
            
            Debug.Log($"SIMULATION: Insulin - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

            if (secondsPassed < AttributeManager.Instance.timeEffectButtons)
            {
                timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                AttributeManager.Instance.isInsulinEffectActive = true;
                if(restTimeEffects < timeDisconnected)
                {
                    _iterationsEffectsInsulin = (int)restTimeEffects / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Insulin Effect Active - Iterations Left: {_iterationsEffectsInsulin}");
                }
                else
                {
                    _iterationsEffectsInsulin = (int)timeDisconnected / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Insulin Effect Active - Iterations Left: {_iterationsEffectsInsulin}");
                }
                
            }
        }

        // Verificación para lastTimeExerciseUsed
        if (AttributeManager.Instance.lastTimeExerciseUsed.HasValue)
        {
            timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeExerciseUsed.Value;
            secondsPassed = (float)timePassed?.TotalSeconds;
            restTimeEffects = AttributeManager.Instance.timeEffectButtons - secondsPassed;

            Debug.Log($"SIMULATION: Exercise - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

            if (secondsPassed < AttributeManager.Instance.timeEffectButtons)
            {
                timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                AttributeManager.Instance.isExerciseEffectActive = true;
                if (restTimeEffects < timeDisconnected)
                {
                    _iterationsEffectsExercise = (int)restTimeEffects / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Exercise Effect Active - Iterations Left: {_iterationsEffectsExercise}");
                }
                else
                {
                    _iterationsEffectsExercise = (int)timeDisconnected / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Exercise Effect Active - Iterations Left: {_iterationsEffectsExercise}");
                }
            }
        }

        // Verificación para lastTimeFoodUsed
        if (AttributeManager.Instance.lastTimeFoodUsed.HasValue)
        {
            timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeFoodUsed.Value;
            secondsPassed = (float)timePassed?.TotalSeconds;
            restTimeEffects =  AttributeManager.Instance.timeEffectButtons - secondsPassed;

            Debug.Log($"SIMULATION: Food - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

            if (secondsPassed < AttributeManager.Instance.timeEffectButtons)
            {
                timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                AttributeManager.Instance.isFoodEffectActive = true;
                if (restTimeEffects < timeDisconnected)
                {
                    _iterationsEffectsFood = (int)restTimeEffects / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Food Effect Active - Iterations Left: {_iterationsEffectsFood}");
                }
                else
                {
                    _iterationsEffectsFood = (int)timeDisconnected / (int)_timeIntervalAI;
                    Debug.Log($"SIMULATION: Food Effect Active - Iterations Left: {_iterationsEffectsFood}");
                }
            }
        }
    }

    private void SimulateIntervals()
    {
        Debug.Log($"SIMULATION: SimulateIntervals() - Total Iterations: {_iterationsTotal}");

        for (int iteration = 0; iteration < _iterationsTotal; iteration++)
        {
            Debug.Log($"SIMULATION: Iteration {iteration + 1}/{_iterationsTotal}");

            GameEventsPetCare.OnExecutingAttributes?.Invoke(_dateTimesQueue.Dequeue());

            _iterationsEffectsInsulin--;
            _iterationsEffectsExercise--;
            _iterationsEffectsFood--;

            Debug.Log($"SIMULATION: Insulin Iterations Left: {_iterationsEffectsInsulin}, Exercise Iterations Left: {_iterationsEffectsExercise}, Food Iterations Left: {_iterationsEffectsFood}");

            if (_iterationsEffectsInsulin <= 0 && !_isInsulinActivated)
            {
                AttributeManager.Instance.isInsulinEffectActive = false;
                Debug.Log("SIMULATION: Insulin Effect Deactivated");
            }
            if (_iterationsEffectsExercise <= 0 && !_isExerciseActivated)
            {
                AttributeManager.Instance.isExerciseEffectActive = false;
                Debug.Log("SIMULATION: Exercise Effect Deactivated");
            }
            if (_iterationsEffectsFood <= 0 && !_isFoodActivated)
            {
                AttributeManager.Instance.isFoodEffectActive = false;
                Debug.Log("SIMULATION: Food Effect Deactivated");
            }
        }
    }
}
