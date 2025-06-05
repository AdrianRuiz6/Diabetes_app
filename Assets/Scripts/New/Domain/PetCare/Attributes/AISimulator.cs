using Master.Domain.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Master.Persistence;
using Master.Domain.Settings;
using Master.Persistence.Connection;

namespace Master.Domain.PetCare
{
    public class AISimulator : MonoBehaviour
    {
        public static AISimulator Instance;

        private int _iterationsTotal;
        private float _initialTimerSeconds;
        private DateTime _currentIterationStartTime;

        private int _iterationsEffectsInsulin;
        private bool _isInsulinActivated;
        private int _iterationsEffectsExercise;
        private bool _isExerciseActivated;
        private int _iterationsEffectsFood;
        private bool _isFoodActivated;

        private float _timeIterationAI;

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
            _timeIterationAI = AttributeSchedule.Instance.UpdateInterval;
            Debug.Log($"SIMULATION: Start Simulation - Time Interval: {_timeIterationAI} seconds");

            CalculateIterations();
            SimulateIterations();
            AttributeSchedule.Instance.UpdateTimer(_initialTimerSeconds, _currentIterationStartTime);
        }

        private void CalculateIterations()
        {
            DateTime lastIterationStartTime = DateTime.Now;
            if (DataStorage_Connection.LoadIsFirstUsage())
            {
                DateTime currentCheckedTime = DateTime.Now.Date.AddHours(SettingsManager.Instance.initialTime.Hours);
                while (currentCheckedTime <= DateTime.Now)
                {
                    currentCheckedTime = currentCheckedTime.AddSeconds(AttributeSchedule.Instance.UpdateInterval);
                }

                lastIterationStartTime = currentCheckedTime.AddSeconds(-AttributeSchedule.Instance.UpdateInterval);
            }
            else
            {
                lastIterationStartTime = DataStorage_Connection.LoadLastIterationStartTime();
            }
            DateTime? currentTime = DateTime.Now;
            TimeSpan? timePassed;
            float secondsPassed;
            float restTimeEffects;
            float timeDisconnected;
            DateTime? lastTimeDisconnected = DataStorage_Connection.LoadDisconnectionDate();

            _isInsulinActivated = AttributeManager.Instance.isInsulinEffectActive;
            _isExerciseActivated = AttributeManager.Instance.isExerciseEffectActive;
            _isFoodActivated = AttributeManager.Instance.isFoodEffectActive;

            // Cálculo de las iteraciones e inicio del tiemer de la IA de los atributos.
            Debug.Log($"SIMULATION: Current Time: {currentTime}");

            // Se calculan los intervalos sucedidos mientras la aplicación estaba desconectada y reinicio del timer de la simulación.

            // Se calcula el tiempo transcurrido desde la última vez que se inició un intervalo hasta ahora.
            TimeSpan timePassedSinceLastIteration = DateTime.Now - lastIterationStartTime;
            secondsPassed = (float)timePassedSinceLastIteration.TotalSeconds;

            // Si no ha pasado suficiente tiempo para completar un intervalo.
            if (secondsPassed < _timeIterationAI)
            {
                _iterationsTotal = 0;
                _initialTimerSeconds = _timeIterationAI - secondsPassed;
                _currentIterationStartTime = lastIterationStartTime;
            }
            // Si ha ha pasado suficiente tiempo para completar un intervalo.
            else
            {
                _iterationsTotal = (int)(secondsPassed / _timeIterationAI);
                _initialTimerSeconds = _timeIterationAI - (secondsPassed % _timeIterationAI);
                _currentIterationStartTime = lastIterationStartTime.AddSeconds(_iterationsTotal * _timeIterationAI);

                // Se guardan las fechas en la lista de fechas y se ajustan al inicio del rango de tiempo si hace falta.
                DateTime dateTimeToIntroduce = lastIterationStartTime;
                bool isPreviousDateTimeToIntroduceInRange = SettingsManager.Instance.IsInRange(dateTimeToIntroduce.TimeOfDay);
                bool isCurrentDateTimeToIntroduceInRange = false;
                for (int iterations = 0; iterations < _iterationsTotal; iterations++)
                {
                    dateTimeToIntroduce = dateTimeToIntroduce.AddSeconds(_timeIterationAI);
                    isCurrentDateTimeToIntroduceInRange = SettingsManager.Instance.IsInRange(dateTimeToIntroduce.TimeOfDay);

                    if (!isPreviousDateTimeToIntroduceInRange && isCurrentDateTimeToIntroduceInRange)
                    {
                        dateTimeToIntroduce = dateTimeToIntroduce.Date.AddHours(SettingsManager.Instance.initialTime.Hours);
                    }
                    _dateTimesQueue.Enqueue(dateTimeToIntroduce);

                    isPreviousDateTimeToIntroduceInRange = isCurrentDateTimeToIntroduceInRange;
                }

                Debug.Log($"SIMULATION: Iteraciones totales pasadas desde el último intervalo: {_iterationsTotal}");
                Debug.Log($"SIMULATION: Segundos restantes para el próximo intervalo: {_initialTimerSeconds}");
                Debug.Log($"SIMULATION: Tiempo de inicio de la iteracion actual: {_currentIterationStartTime}");
            }


            // Cálculo de la duración en intervalos del efecto de la insulina.
            if (AttributeManager.Instance.lastTimeInsulinUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeInsulinUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = AttributeManager.Instance.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Insulin - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < AttributeManager.Instance.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    AttributeManager.Instance.isInsulinEffectActive = true;
                    if (restTimeEffects < timeDisconnected)
                    {
                        _iterationsEffectsInsulin = (int)restTimeEffects / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Insulin Effect Active - Iterations Left: {_iterationsEffectsInsulin}");
                    }
                    else
                    {
                        _iterationsEffectsInsulin = (int)timeDisconnected / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Insulin Effect Active - Iterations Left: {_iterationsEffectsInsulin}");
                    }

                }
            }

            // Cálculo de la duración en intervalos del efecto del ejercicio.
            if (AttributeManager.Instance.lastTimeExerciseUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeExerciseUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = AttributeManager.Instance.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Exercise - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < AttributeManager.Instance.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    AttributeManager.Instance.isExerciseEffectActive = true;
                    if (restTimeEffects < timeDisconnected)
                    {
                        _iterationsEffectsExercise = (int)restTimeEffects / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Exercise Effect Active - Iterations Left: {_iterationsEffectsExercise}");
                    }
                    else
                    {
                        _iterationsEffectsExercise = (int)timeDisconnected / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Exercise Effect Active - Iterations Left: {_iterationsEffectsExercise}");
                    }
                }
            }

            // Cálculo de la duración en intervalos del efecto de la comida.
            if (AttributeManager.Instance.lastTimeFoodUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - AttributeManager.Instance.lastTimeFoodUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = AttributeManager.Instance.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Food - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < AttributeManager.Instance.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    AttributeManager.Instance.isFoodEffectActive = true;
                    if (restTimeEffects < timeDisconnected)
                    {
                        _iterationsEffectsFood = (int)restTimeEffects / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Food Effect Active - Iterations Left: {_iterationsEffectsFood}");
                    }
                    else
                    {
                        _iterationsEffectsFood = (int)timeDisconnected / (int)_timeIterationAI;
                        Debug.Log($"SIMULATION: Food Effect Active - Iterations Left: {_iterationsEffectsFood}");
                    }
                }
            }
        }

        private void SimulateIterations()
        {
            Debug.Log($"SIMULATION: SimulateIntervals() - Total Iterations: {_iterationsTotal}");

            for (int iteration = 0; iteration < _iterationsTotal; iteration++)
            {
                Debug.Log($"SIMULATION: Iteration {iteration + 1}/{_iterationsTotal}");

                GameEvents_PetCare.OnExecutingAttributes?.Invoke(_dateTimesQueue.Dequeue());

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
}