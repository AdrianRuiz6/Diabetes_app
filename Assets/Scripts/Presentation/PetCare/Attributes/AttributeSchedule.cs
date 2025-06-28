using System;
using UnityEngine;
using Master.Domain.PetCare;
using Master.Infrastructure;
using Master.Domain.GameEvents;
using System.Collections;

namespace Master.Presentation.PetCare
{
    public class AttributeSchedule : MonoBehaviour
    {
        IAISimulatorManager _aiSimulatorManager;
        IPetCareManager _petCareManager;

        private DateTime _nextExecutionTime;
        private DateTime _previousExecutionTime;
        private float _interval;
        private bool isUpdating = false;

        private void Awake()
        {
            GameEvents_PetCare.OnFinishedExecutionAttributesBTree += FinishSimulationAttribute;
        }

        private void OnDestroy()
        {
            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= FinishSimulationAttribute;
        }

        private void Start()
        {
            _aiSimulatorManager = ServiceLocator.Instance.GetService<IAISimulatorManager>();
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();

            _interval = _petCareManager.updateIntervalBTree;

            // Simulacion inicial y inicio del timer
            _aiSimulatorManager.StartSimulation();
            StartCoroutine(WaitForSimulationToEnd());
        }

        private void Update()
        {
            if (isUpdating == true && DateTime.Now >= _nextExecutionTime)
            {
                _petCareManager.ExecuteAttributesBTree();
                ScheduleNextBTreeCall();
            }
        }

        private IEnumerator WaitForSimulationToEnd()
        {
            yield return new WaitUntil(() => _aiSimulatorManager.iterationsTotal == 0);

            _nextExecutionTime = _aiSimulatorManager.currentIterationFinishTime;
            isUpdating = true;

        }

        private void ScheduleNextBTreeCall()
        {
            _previousExecutionTime = _nextExecutionTime;
            _nextExecutionTime = _previousExecutionTime.AddSeconds(_interval);
            _petCareManager.SetNextIterationStartTime(_nextExecutionTime);
        }

        private void FinishSimulationAttribute()
        {
            _aiSimulatorManager.FinishSimulationAttribute();
        }
    }
}