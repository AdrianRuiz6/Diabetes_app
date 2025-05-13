using Master.Domain.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Master.Presentation.PetCare
{
    public class UI_HungerBar : MonoBehaviour
    {
        [SerializeField] private Image _hungerFillBar;
        [SerializeField] private Gradient _hungerColorGradient;

        [SerializeField] private GameObject _valueObject;

        [SerializeField] private Vector2 _minPosition;
        [SerializeField] private Vector2 _maxPosition;

        private int _currentHungerValue;
        private int _maxHunger = 350;
        private int _minHunger = 20;

        void Awake()
        {
            GameEvents_PetCare.OnModifyHunger += UpdateVisualBar;
        }

        void OnDestroy()
        {
            GameEvents_PetCare.OnModifyHunger -= UpdateVisualBar;
        }

        private void UpdateVisualBar(int newHungerValue, DateTime? currentDateTime, bool isRestarting = false)
        {
            _currentHungerValue = Mathf.Clamp(_currentHungerValue + newHungerValue, 0, 100);
            float amount = _currentHungerValue / _maxHunger;

            float normalizedValue = Mathf.InverseLerp(_minHunger, _maxHunger, _currentHungerValue);
            Vector2 newPosition = Vector2.Lerp(_minPosition, _maxPosition, normalizedValue);
            _valueObject.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            _hungerFillBar.color = _hungerColorGradient.Evaluate(amount);
        }
    }
}