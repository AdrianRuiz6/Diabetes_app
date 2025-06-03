using Master.Domain.GameEvents;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.PetCare;

namespace Master.Presentation.PetCare
{
    public enum AttributeType
    {
        Glycemia,
        Activity,
        Hunger
    }
}

namespace Master.Presentation.PetCare
{
    public class UI_Attribute : MonoBehaviour
    {
        [SerializeField] private AttributeType _attributeType;

        private float _minValue;
        private float _maxValue;

        private List<AttributeState> _attributeStates;
        private Slider _slider;
        private Image _sliderBackground;
        private TextMeshProUGUI _value_TXT;
        private string _unit;
        private float _currentValue;

        void Awake()
        {
            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    GameEvents_PetCare.OnModifyGlycemia += UpdateVisualBar;
                    break;
                case AttributeType.Activity:
                    GameEvents_PetCare.OnModifyActivity += UpdateVisualBar;
                    break;
                case AttributeType.Hunger:
                    GameEvents_PetCare.OnModifyHunger += UpdateVisualBar;
                    break;
            }
        }

        void OnDestroy()
        {
            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    GameEvents_PetCare.OnModifyGlycemia -= UpdateVisualBar;
                    break;
                case AttributeType.Activity:
                    GameEvents_PetCare.OnModifyActivity -= UpdateVisualBar;
                    break;
                case AttributeType.Hunger:
                    GameEvents_PetCare.OnModifyHunger -= UpdateVisualBar;
                    break;
            }
        }

        void Start()
        {
            _slider = GetComponentInChildren<Slider>();
            _value_TXT = GetComponentInChildren<TextMeshProUGUI>();
            _slider.wholeNumbers = true;

            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    _currentValue = AttributeManager.Instance.glycemiaValue;
                    _attributeStates = AttributeManager.Instance.GlycemiaRangeStates;
                    _minValue = AttributeManager.Instance.minGlycemiaValue;
                    _maxValue = AttributeManager.Instance.maxGlycemiaValue;
                    _unit = "mg/dL";
                    break;
                case AttributeType.Activity:
                    _currentValue = AttributeManager.Instance.activityValue;
                    _attributeStates = AttributeManager.Instance.ActivityRangeStates;
                    _minValue = AttributeManager.Instance.minActivityValue;
                    _maxValue = AttributeManager.Instance.maxActivityValue;
                    _unit = "%";
                    break;
                case AttributeType.Hunger:
                    _currentValue = AttributeManager.Instance.hungerValue;
                    _attributeStates = AttributeManager.Instance.HungerRangeStates;
                    _minValue = AttributeManager.Instance.minHungerValue;
                    _maxValue = AttributeManager.Instance.maxHungerValue;
                    _unit = "%";
                    break;
            }

            Transform background = transform.Find("Bar/Background");
            _sliderBackground = background.GetComponent<Image>();
            _slider.minValue = _minValue;
            _slider.maxValue = _maxValue;
            _value_TXT.text = $"{_slider.value} {_unit}";

            _slider.value = _currentValue;
            InitializeVisualBar();
            _slider.onValueChanged.AddListener(ChangeValue);
        }

        private void ChangeValue(float arg0)
        {
            _slider.SetValueWithoutNotify(_currentValue);
        }

        private void InitializeVisualBar()
        {
            _currentValue = Mathf.Clamp(_slider.value, _minValue, _maxValue);
            _slider.SetValueWithoutNotify(_currentValue);
            _value_TXT.text = $"{_slider.value} {_unit}";

            foreach (AttributeState state in _attributeStates)
            {
                if (_slider.value >= state.MinValue && _slider.value <= state.MaxValue)
                {
                    _sliderBackground.color = state.StateColor;
                    break;
                }
            }
        }

        private void UpdateVisualBar(int additionalValue, DateTime? currentDataTime, bool isRestarting = false)
        {
            _currentValue = Mathf.Clamp(_slider.value + additionalValue, _minValue, _maxValue);
            _slider.SetValueWithoutNotify(_currentValue);
            _value_TXT.text = $"{_slider.value} {_unit}";

            foreach (AttributeState state in _attributeStates)
            {
                if (_slider.value >= state.MinValue && _slider.value <= state.MaxValue)
                {
                    _sliderBackground.color = state.StateColor;
                    break;
                }
            }
        }
    }
}