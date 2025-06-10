using System;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Settings;
using Master.Domain.Connection;
using Master.Domain.GameEvents;

namespace Master.Domain.PetCare
{
    public class AISimulatorManager : IAISimulatorManager
    {
        private int _iterationsTotal;
        public float initialTimerSeconds { get; private set; }
        private DateTime currentIterationStartTime;

        private int _iterationsEffectsInsulin;
        private int _iterationsEffectsExercise;
        private int _iterationsEffectsFood;

        private float _timeIterationAI;

        private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

        private IPetCareManager _petCareManager;
        private IConnectionManager _connectionManager;
        private ISettingsManager _settingsManager;

        public AISimulatorManager(IPetCareManager petCareManager, IConnectionManager connectionManager, ISettingsManager settingsManager)
        {
            _petCareManager = petCareManager;
            _connectionManager = connectionManager;
            _settingsManager = settingsManager;

            _timeIterationAI = _petCareManager.updateIntervalBTree;
            CalculateIterations();
        }

        public void Simulate()
        {
            SimulateIterations();
            _petCareManager.SetLastIterationStartTime(currentIterationStartTime);
        }

        private void CalculateIterations()
        {
            DateTime lastIterationStartTime = DateTime.Now;
            if (_connectionManager.isFirstUsage)
            {
                DateTime currentCheckedTime = DateTime.Now.Date.AddHours(_settingsManager.initialTime.Hours);
                while (currentCheckedTime <= DateTime.Now)
                {
                    currentCheckedTime = currentCheckedTime.AddSeconds(_petCareManager.updateIntervalBTree);
                }

                lastIterationStartTime = currentCheckedTime.AddSeconds(-_petCareManager.updateIntervalBTree);
            }
            else
            {
                lastIterationStartTime = _petCareManager.lastIterationBTreeStartTime;
            }
            DateTime? currentTime = DateTime.Now;
            TimeSpan? timePassed;
            float secondsPassed;
            float restTimeEffects;
            float timeDisconnected;
            DateTime? lastTimeDisconnected = _connectionManager.lastDisconnectionDateTime;

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
                initialTimerSeconds = _timeIterationAI - secondsPassed;
                currentIterationStartTime = lastIterationStartTime;
            }
            // Si ha ha pasado suficiente tiempo para completar un intervalo.
            else
            {
                _iterationsTotal = (int)(secondsPassed / _timeIterationAI);
                initialTimerSeconds = _timeIterationAI - (secondsPassed % _timeIterationAI);
                currentIterationStartTime = lastIterationStartTime.AddSeconds(_iterationsTotal * _timeIterationAI);

                // Se guardan las fechas en la lista de fechas y se ajustan al inicio del rango de tiempo si hace falta.
                DateTime dateTimeToIntroduce = lastIterationStartTime;
                bool isPreviousDateTimeToIntroduceInRange = _settingsManager.IsInRange(dateTimeToIntroduce.TimeOfDay);
                bool isCurrentDateTimeToIntroduceInRange = false;
                for (int iterations = 0; iterations < _iterationsTotal; iterations++)
                {
                    dateTimeToIntroduce = dateTimeToIntroduce.AddSeconds(_timeIterationAI);
                    isCurrentDateTimeToIntroduceInRange = _settingsManager.IsInRange(dateTimeToIntroduce.TimeOfDay);

                    if (!isPreviousDateTimeToIntroduceInRange && isCurrentDateTimeToIntroduceInRange)
                    {
                        dateTimeToIntroduce = dateTimeToIntroduce.Date.AddHours(_settingsManager.initialTime.Hours);
                    }
                    _dateTimesQueue.Enqueue(dateTimeToIntroduce);

                    isPreviousDateTimeToIntroduceInRange = isCurrentDateTimeToIntroduceInRange;
                }

                Debug.Log($"SIMULATION: Iteraciones totales pasadas desde el último intervalo: {_iterationsTotal}");
                Debug.Log($"SIMULATION: Segundos restantes para el próximo intervalo: {initialTimerSeconds}");
                Debug.Log($"SIMULATION: Tiempo de inicio de la iteracion actual: {currentIterationStartTime}");
            }


            // Cálculo de la duración en intervalos del efecto de la insulina.
            if (_petCareManager.lastTimeInsulinUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - _petCareManager.lastTimeInsulinUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = _petCareManager.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Insulin - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < _petCareManager.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    _petCareManager.isInsulinEffectActive = true;
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
            if (_petCareManager.lastTimeExerciseUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - _petCareManager.lastTimeExerciseUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = _petCareManager.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Exercise - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < _petCareManager.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    _petCareManager.isExerciseEffectActive = true;
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
            if (_petCareManager.lastTimeFoodUsed.HasValue)
            {
                timePassed = lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime) - _petCareManager.lastTimeFoodUsed.Value;
                secondsPassed = (float)timePassed?.TotalSeconds;
                restTimeEffects = _petCareManager.timeEffectActions - secondsPassed;

                Debug.Log($"SIMULATION: Food - Time Passed (lastTimeUsed -> DisconectionTIme): {secondsPassed}, Rest of the time: {restTimeEffects}");

                if (secondsPassed < _petCareManager.timeEffectActions)
                {
                    timeDisconnected = (float)(currentTime - lastTimeDisconnected.GetValueOrDefault((DateTime)currentTime))?.TotalSeconds;
                    _petCareManager.isFoodEffectActive = true;
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

                GameEvents_PetCare.OnExecuteAttributesBTree?.Invoke(_dateTimesQueue.Dequeue());

                _iterationsEffectsInsulin--;
                _iterationsEffectsExercise--;
                _iterationsEffectsFood--;

                Debug.Log($"SIMULATION: Insulin Iterations Left: {_iterationsEffectsInsulin}, Exercise Iterations Left: {_iterationsEffectsExercise}, Food Iterations Left: {_iterationsEffectsFood}");

                if (_iterationsEffectsInsulin <= 0 && !_petCareManager.isInsulinEffectActive)
                {
                    _petCareManager.isInsulinEffectActive = false;
                    Debug.Log("SIMULATION: Insulin Effect Deactivated");
                }
                if (_iterationsEffectsExercise <= 0 && !_petCareManager.isExerciseEffectActive)
                {
                    _petCareManager.isExerciseEffectActive = false;
                    Debug.Log("SIMULATION: Exercise Effect Deactivated");
                }
                if (_iterationsEffectsFood <= 0 && !_petCareManager.isFoodEffectActive)
                {
                    _petCareManager.isFoodEffectActive = false;
                    Debug.Log("SIMULATION: Food Effect Deactivated");
                }
            }
        }
    }
}