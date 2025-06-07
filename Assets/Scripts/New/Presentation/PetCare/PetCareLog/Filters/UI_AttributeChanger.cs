using System;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.GameEvents;

namespace Master.Presentation.PetCare.Log
{
    public class UI_AttributeChanger : MonoBehaviour
    {
        [SerializeField] private Button _glycemiaButton;
        [SerializeField] private Button _activityButton;
        [SerializeField] private Button _hungerButton;

        void Start()
        {
            _glycemiaButton.onClick.AddListener(UpdateGlycemia);
            _activityButton.onClick.AddListener(UpdateActivity);
            _hungerButton.onClick.AddListener(UpdateHunger);
        }

        private void UpdateGlycemia()
        {
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Glycemia);
        }
        private void UpdateActivity()
        {
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Activity);
        }
        private void UpdateHunger()
        {
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Hunger);
        }
    }
}