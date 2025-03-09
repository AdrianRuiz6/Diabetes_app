using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GlycemiaBar : MonoBehaviour
{
    [SerializeField] private Image _glucoseFillBar;
    [SerializeField] private Gradient _glucoseColorGradient;

    [SerializeField] private GameObject _valueObject;

    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    private int _currentHungerValue;
    private int _maxGlucose = 100;
    private int _minGlucose = 0;

    void Awake()
    {
        GameEventsPetCare.OnModifyGlycemia += UpdateVisualBar;
    }

    void OnDestroy()
    {
        GameEventsPetCare.OnModifyGlycemia -= UpdateVisualBar;
    }

    private void UpdateVisualBar(int newGlucoseValue, DateTime? currentDataTime, bool isRestarting = false)
    {
        _currentHungerValue = Mathf.Clamp(_currentHungerValue + newGlucoseValue, 0, 100);
        float amount = _currentHungerValue / _maxGlucose;

        float normalizedValue = Mathf.InverseLerp(_minGlucose, _maxGlucose, _currentHungerValue);
        Vector2 newPosition = Vector2.Lerp(_minPosition, _maxPosition, normalizedValue);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        _glucoseFillBar.color = _glucoseColorGradient.Evaluate(amount);
    }
}
