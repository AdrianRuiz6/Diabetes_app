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

        private bool isUpdating = false;

        private void Awake()
        {
            GameEvents_PetCare.OnFinishedExecutionAttributesBTree += FinishSimulationAttribute;
        }


#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= FinishSimulationAttribute;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= FinishSimulationAttribute;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_PetCare.OnFinishedExecutionAttributesBTree -= FinishSimulationAttribute;
        }
#endif

        private void Start()
        {
            _aiSimulatorManager = ServiceLocator.Instance.GetService<IAISimulatorManager>();
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();

            // Simulacion inicial y inicio del timer
            _aiSimulatorManager.StartSimulation();
            StartCoroutine(WaitForSimulationToEnd());
        }

        private void Update()
        {
            if (isUpdating == true && DateTime.Now >= _petCareManager.nextIterationStartTime)
            {
                _petCareManager.ExecuteAttributesBTree();
                _petCareManager.ScheduleNextBTCall();
            }
        }

        private IEnumerator WaitForSimulationToEnd()
        {
            yield return new WaitUntil(() => _aiSimulatorManager.iterationsTotal == 0);

            isUpdating = true;
        }

        private void FinishSimulationAttribute()
        {
            _aiSimulatorManager.FinishSimulationAttribute();
        }
    }
}