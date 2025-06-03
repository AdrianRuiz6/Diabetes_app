using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        private DateTime _currentDate = DateTime.Now.Date;

        void Start()
        {
            _glycemiaButton.onClick.AddListener(UpdateGlycemia);
            _activityButton.onClick.AddListener(UpdateActivity);
            _hungerButton.onClick.AddListener(UpdateHunger);
        }

        private void UpdateGlycemia()
        {
            GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Glycemia);
        }
        private void UpdateActivity()
        {
            GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Activity);
        }
        private void UpdateHunger()
        {
            GameEvents_PetCareLog.OnUpdatedAttributeLog?.Invoke(GraphFilter.Hunger);
        }
    }
}