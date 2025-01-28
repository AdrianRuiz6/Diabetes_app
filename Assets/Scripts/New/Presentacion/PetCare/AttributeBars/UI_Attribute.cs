using Master.Domain.Events;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AttributeType
{
    Glycemia,
    Activity,
    Hunger
}

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
                GameEventsPetCare.OnModifyGlycemia += UpdateVisualBar;
                _currentValue = DataStorage.LoadGlycemia();
                _attributeStates = AttributeManager.Instance.GlycemiaRangeStates;
                _minValue = AttributeManager.Instance.minGlycemiaValue;
                _maxValue = AttributeManager.Instance.maxGlycemiaValue;
                _unit = "mg/dL";
                break;
            case AttributeType.Activity:
                GameEventsPetCare.OnModifyActivity += UpdateVisualBar;
                _currentValue = DataStorage.LoadActivity();
                _attributeStates = AttributeManager.Instance.ActivityRangeStates;
                _minValue = AttributeManager.Instance.minActivityValue;
                _maxValue = AttributeManager.Instance.maxActivityValue;
                _unit = "%";
                break;
            case AttributeType.Hunger:
                GameEventsPetCare.OnModifyHunger += UpdateVisualBar;
                _currentValue = DataStorage.LoadHunger();
                _attributeStates = AttributeManager.Instance.HungerRangeStates;
                _minValue = AttributeManager.Instance.minHungerValue;
                _maxValue = AttributeManager.Instance.maxHungerValue;
                _unit = "%";
                break;
        }
    }

    void OnDestroy()
    {
        switch (_attributeType)
        {
            case AttributeType.Glycemia:
                GameEventsPetCare.OnModifyGlycemia -= UpdateVisualBar;
                break;
            case AttributeType.Activity:
                GameEventsPetCare.OnModifyActivity -= UpdateVisualBar;
                break;
            case AttributeType.Hunger:
                GameEventsPetCare.OnModifyHunger -= UpdateVisualBar;
                break;
        }
    }

    void Start()
    {
        _slider = GetComponentInChildren<Slider>();
        _value_TXT = GetComponentInChildren<TextMeshProUGUI>();
        _slider.wholeNumbers = true;

        Transform background = transform.Find("Bar/Background");
        _sliderBackground = background.GetComponent<Image>();
        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;
        _value_TXT.text = $"{_slider.value} {_unit}";

        _slider.value = _currentValue;
        UpdateVisualBar(0, null);
        _slider.onValueChanged.AddListener(ChangeValue);
    }

    private void ChangeValue(float arg0)
    {
        _slider.SetValueWithoutNotify(_currentValue);
    }

    private void UpdateVisualBar(int additionalValue, DateTime? currentDataTime)
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
