using System;
using System.Collections;
using UnityEngine;
using Master.Domain.PetCare;

namespace Master.Presentation.PetCare
{
    public class AttributeSchedule : MonoBehaviour
    {
        IAISimulatorManager _aiSimulatorManager;
        IPetCareManager _petCareManager;

        private void Start()
        {
            _aiSimulatorManager = ServiceLocator.Instance.GetService<IAISimulatorManager>();
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();

            _aiSimulatorManager.Simulate();
            StartCoroutine(TimerAttributes(_aiSimulatorManager.initialTimerSeconds));
        }

        private IEnumerator TimerAttributes(float timeFirstIteration)
        {
            yield return new WaitForSeconds(timeFirstIteration);
            _petCareManager.ExecuteAttributesBTree();

            while (true)
            {
                _petCareManager.SetLastIterationStartTime(DateTime.Now);
                yield return new WaitForSeconds(_petCareManager.updateIntervalBTree);
                _petCareManager.ExecuteAttributesBTree();
            }
        }
    }
}