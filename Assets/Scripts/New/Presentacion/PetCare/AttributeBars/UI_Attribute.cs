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

    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color intermediateColor = Color.yellow;
    [SerializeField] private Color badColor = Color.red;

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
                _currentValue = DataStorage.LoadGlycemia();
                break;
            case AttributeType.Activity:
                GameEventsPetCare.OnModifyActivity += UpdateVisualBar;
                _currentValue = DataStorage.LoadActivity();
                break;
            case AttributeType.Hunger:
                GameEventsPetCare.OnModifyHunger += UpdateVisualBar;
                _currentValue = DataStorage.LoadHunger();
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
        _negativeIntermediateValue = _negativeGoodValue - _intermediateValueAmount;

        switch (_attributeType)
        {
            case AttributeType.Glycemia:
                _currentValue = DataStorage.LoadGlycemia();
                break;
            case AttributeType.Activity:
                _currentValue = DataStorage.LoadActivity();
                break;
            case AttributeType.Hunger:
                _currentValue = DataStorage.LoadHunger();
                break;
        }

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

        _value_TXT.text = _slider.value.ToString();

        if (_slider.value >= _negativeGoodValue && _slider.value <= _positiveGoodValue)
        {
            _sliderBackground.color = goodColor;
        }
        else if ((_slider.value >= _negativeIntermediateValue && _slider.value < _negativeGoodValue) ||
                 (_slider.value > _positiveGoodValue && _slider.value <= _positiveIntermediateValue))
        {
            _sliderBackground.color = intermediateColor;
        }
        else
        {
            
            _sliderBackground.color = badColor;
        }
    }
}
