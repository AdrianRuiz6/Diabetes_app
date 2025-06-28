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
        public int iterationsTotal { get; private set; }
        public DateTime currentIterationFinishTime { get; private set; }

        private int _iterationsEffectsInsulin;
        private int _iterationsEffectsExercise;
        private int _iterationsEffectsFood;

        private float _timeIterationAI;

        private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

        private bool _isSimulating = true;
        private int _attributesCountFinishedSimulation = 3;

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

        public void StartSimulation()
        {
            if(iterationsTotal > 0)
            {
                SimulateIteration();
            }
            else{
                _isSimulating = false;
            }
        }

        private void CalculateIterations()
        {
            // 1. Se calcula la hora final de la ultima iteracion
            DateTime lastIterationFinishTime;

            if (_connectionManager.isFirstUsage)
            {
                DateTime rangeStartTime = DateTime.Now.Date.AddHours(_settingsManager.initialTime.Hours);
                double secondsSinceStart = (DateTime.Now - rangeStartTime).TotalSeconds;

                long alignedSeconds = (long)(secondsSinceStart / _timeIterationAI) * (long)_timeIterationAI;
                lastIterationFinishTime = rangeStartTime.AddSeconds(alignedSeconds + _timeIterationAI);
            }
            else
            {
                lastIterationFinishTime = _petCareManager.nextIterationStartTime;
            }

            // 2. Se calculan cuántas iteraciones hay que simular
            float secondsPassedSinceLastIteration = (float)(DateTime.Now - lastIterationFinishTime.AddSeconds(-_timeIterationAI)).TotalSeconds;
            iterationsTotal = Mathf.FloorToInt(secondsPassedSinceLastIteration / _timeIterationAI);

            // 3. Se generan las horas de las iteraciones a simular
            DateTime dateTimeToIntroduce = lastIterationFinishTime;
            for (int i = 0; i < iterationsTotal; i++)
            {
                _dateTimesQueue.Enqueue(dateTimeToIntroduce);
                dateTimeToIntroduce = dateTimeToIntroduce.AddSeconds(_timeIterationAI);
            }

            // 4. Se actualiza el tiempo de la proxima simulacino en tiempo real
            currentIterationFinishTime = dateTimeToIntroduce;

            // 5. Se calculan las iteraciones en las que los efectos de las acciones están habilitados
            if (_connectionManager.isFirstUsage)
            {
                _iterationsEffectsInsulin = 0;
                _iterationsEffectsExercise = 0;
                _iterationsEffectsFood = 0;
            }
            else
            {
                CalculateEffectIterations(_petCareManager.insulinEffectsEndTime, ref _iterationsEffectsInsulin);
                CalculateEffectIterations(_petCareManager.exerciseEffectsEndTime, ref _iterationsEffectsExercise);
                CalculateEffectIterations(_petCareManager.foodEffectsEndTime, ref _iterationsEffectsFood);
            }
        }

        private void CalculateEffectIterations(DateTime endTime, ref int iterations)
        {
            DateTime? lastTimeDisconnected = _connectionManager.lastDisconnectionDateTime;
            if (endTime > lastTimeDisconnected)
            {
                float remainingTimeEffectsActive = (float)(endTime - lastTimeDisconnected).Value.TotalSeconds;
                iterations = (int)(remainingTimeEffectsActive / _timeIterationAI);
            }
        }

        private void SimulateIteration()
        {
            bool isEffectsInsulinActive = false;
            bool isExerciseEffectActive = false;
            bool isFoodEffectActive = false;

            if (_iterationsEffectsInsulin > 0)
            {
                isEffectsInsulinActive = true;
            }
            if (_iterationsEffectsExercise > 0)
            {
                isExerciseEffectActive = true;
            }
            if (_iterationsEffectsFood > 0)
            {
                isFoodEffectActive = true;
            }

            _iterationsEffectsInsulin--;
            _iterationsEffectsExercise--;
            _iterationsEffectsFood--;

            AttributeUpdateIntervalInfo currentIntervalInfo = new AttributeUpdateIntervalInfo
                (
                    _dateTimesQueue.Dequeue(),
                    _petCareManager.glycemiaValue,
                    _petCareManager.energyValue,
                    _petCareManager.hungerValue,
                    isEffectsInsulinActive,
                    isExerciseEffectActive,
                    isFoodEffectActive
                );

            _attributesCountFinishedSimulation = 0;
            GameEvents_PetCare.OnExecuteAttributesBTree?.Invoke(currentIntervalInfo);
        }

        public void FinishSimulationAttribute()
        {
            if (!_isSimulating)
                return;

            _attributesCountFinishedSimulation++;

            if (_attributesCountFinishedSimulation == 3)
            {
                iterationsTotal--;

                if(iterationsTotal == 0)
                {
                    _isSimulating = false;
                    _petCareManager.SetNextIterationStartTime(currentIterationFinishTime);
                }
                else
                {
                    SimulateIteration();
                }
            }
        }
    }
}