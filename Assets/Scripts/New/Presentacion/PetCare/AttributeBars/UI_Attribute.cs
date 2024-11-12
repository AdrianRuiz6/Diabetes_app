using Master.Domain.Events;
using System;
using System.Collections;
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
    [SerializeField] private int _goodValueAmount;
    [SerializeField] private int _intermediateValueAmount;
    private int _positiveGoodValue;
    private int _negativeGoodValue;
    private int _positiveIntermediateValue;
    private int _negativeIntermediateValue;

    [SerializeField] private int _minValue;
    [SerializeField] private int _maxValue;

    private Slider _slider;
    private Image _sliderBackground;
    private TextMeshProUGUI _value_TXT;
    private float _currentValue;

    void Awake()
    {
        switch (_attributeType)
        {
            case AttributeType.Glycemia:
                GameEventsPetCare.OnModifyGlycemia += UpdateVisualBar;
                break;
            case AttributeType.Activity:
                GameEventsPetCare.OnModifyActivity += UpdateVisualBar;
                break;
            case AttributeType.Hunger:
                GameEventsPetCare.OnModifyHunger += UpdateVisualBar;
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
        _value_TXT.text = _slider.value.ToString();

        int midrange = (_maxValue - _minValue) / 2;
        _positiveGoodValue = midrange + _goodValueAmount;
        _negativeGoodValue = midrange - _goodValueAmount;
        _positiveIntermediateValue = _positiveGoodValue + _intermediateValueAmount;
        _negativeIntermediateValue = _negativeIntermediateValue - _intermediateValueAmount;

        _currentValue = _slider.value;
        _slider.onValueChanged.AddListener(ChangeValue);
    }

    private void ChangeValue(float arg0)
    {
        _slider.SetValueWithoutNotify(_currentValue);
    }

    private void UpdateVisualBar(int additionalValue, DateTime? currentDataTime)
    {
        _currentValue = _slider.value + additionalValue;
        _slider.SetValueWithoutNotify(_currentValue);

        _value_TXT.text = _slider.value.ToString();

        
        if(_slider.value >= _negativeGoodValue && _slider.value <= _positiveGoodValue)
        {
            _sliderBackground.color = Color.green;
        }
        else if (_slider.value >= _negativeIntermediateValue && _slider.value < _negativeGoodValue ||
            _slider.value > _positiveGoodValue && _slider.value <= _positiveIntermediateValue)
        {
            _sliderBackground.color = Color.yellow;
        }
        else
        {
            _sliderBackground.color = Color.red;
        }
    }
}
