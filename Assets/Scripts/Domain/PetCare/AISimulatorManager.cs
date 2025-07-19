using System;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Settings;
using Master.Domain.Connection;
using Master.Domain.GameEvents;
using Master.Domain.BehaviorTree;

namespace Master.Domain.PetCare
{
    public class AISimulatorManager : IAISimulatorManager
    {
        public int iterationsTotal { get; private set; }
        private DateTime _currentIterationFinishTime;

        private int _iterationsEffectsInsulin;
        private int _iterationsEffectsExercise;
        private int _iterationsEffectsFood;

        private float _timeIterationBT;

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

            _timeIterationBT = _petCareManager.updateIntervalBTree;
            CalculateIterations();
        }

        public void StartSimulation()
        {
            if (iterationsTotal > 0)
            {
                SimulateIteration();
            }
            else{
                _isSimulating = false;
                GameEvents_PetCare.OnFinishedSimulation?.Invoke();
                _petCareManager.SetNextIterationStartTime(_currentIterationFinishTime);
            }
        }

        private void CalculateIterations()
        {
            DateTime now = DateTime.Now;

            // 1. Se calcula la hora final de la ultima iteracion
            DateTime lastIterationFinishTime;

            if (_connectionManager.isFirstUsage)
            {
                DateTime rangeStartTime = now.Date.AddHours(_settingsManager.initialTime.Hours);
                double secondsSinceStart = (now - rangeStartTime).TotalSeconds;

                long alignedSeconds = (long)(secondsSinceStart / _timeIterationBT) * (long)_timeIterationBT;
                lastIterationFinishTime = rangeStartTime.AddSeconds(alignedSeconds + _timeIterationBT);
            }
            else
            {
                lastIterationFinishTime = _petCareManager.nextIterationStartTime;
            }

            // 2. Se generan y calculan las iteraciones a simular y las iteraciones activas de las acciones
            iterationsTotal = 0;
            _iterationsEffectsInsulin = 0;
            _iterationsEffectsExercise = 0;
            _iterationsEffectsFood = 0;

            DateTime dateTimeToIntroduce = lastIterationFinishTime;

            while (dateTimeToIntroduce < now)
            {
                TimeSpan time = dateTimeToIntroduce.TimeOfDay;
                DateTime date = dateTimeToIntroduce.Date;

                bool isInTime = _settingsManager.IsInRange(time);
                bool isInCurrentDate = date == _connectionManager.currentConnectionDateTime.Date;
                bool isInLastSessionDate = date == _connectionManager.lastDisconnectionDateTime.Date;

                if ((isInLastSessionDate && isInTime) || (isInCurrentDate && isInTime))
                {
                    _dateTimesQueue.Enqueue(dateTimeToIntroduce);
                    iterationsTotal++;
                    EvaluateEffectIterations(dateTimeToIntroduce);
                }

                dateTimeToIntroduce = dateTimeToIntroduce.AddSeconds(_timeIterationBT);
            }

            // 3. Se actualiza el tiempo de la proxima actualización en tiempo real
            _currentIterationFinishTime = dateTimeToIntroduce;
        }

        private void EvaluateEffectIterations(DateTime currentTime)
        {
            if (_connectionManager.isFirstUsage)
                return;

            if (_petCareManager.insulinEffectsEndTime > currentTime)
            {
                _iterationsEffectsInsulin++;
            }

            if(_petCareManager.exerciseEffectsEndTime > currentTime)
            {
                _iterationsEffectsExercise++;
            }

            if (_petCareManager.foodEffectsEndTime > currentTime)
            {
                _iterationsEffectsFood++;
            }
        }

        private void SimulateIteration()
        {
            bool isInsulinEffectActive = false;
            bool isExerciseEffectActive = false;
            bool isFoodEffectActive = false;

            if (_iterationsEffectsInsulin > 0)
            {
                isInsulinEffectActive = true;
                _iterationsEffectsInsulin--;
            }
            if (_iterationsEffectsExercise > 0)
            {
                isExerciseEffectActive = true;
                _iterationsEffectsExercise--;
            }
            if (_iterationsEffectsFood > 0)
            {
                isFoodEffectActive = true;
                _iterationsEffectsFood--;
            }

            AttributeUpdateIntervalInfo currentIntervalInfo = new AttributeUpdateIntervalInfo
                (
                    _dateTimesQueue.Dequeue(),
                    _petCareManager.glycemiaValue,
                    _petCareManager.energyValue,
                    _petCareManager.hungerValue,
                    isInsulinEffectActive,
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
                    GameEvents_PetCare.OnFinishedSimulation?.Invoke();
                    _petCareManager.SetNextIterationStartTime(_currentIterationFinishTime);
                }
                else
                {
                    SimulateIteration();
                }
            }
        }
    }
}