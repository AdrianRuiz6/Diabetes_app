using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActivityBar : MonoBehaviour
{
    [SerializeField] private Image _activityFillBar;
    [SerializeField] private Gradient _activityColorGradient;

    [SerializeField] private GameObject _valueObject;

    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    private int _currentActivityValue;
    private int _maxActivity = 100;
    private int _minActivity = 0;

    void Awake()
    {
        GameEventsPetCare.OnModifyActivity += UpdateVisualBar;
    }

    void OnDestroy()
    {
        GameEventsPetCare.OnModifyActivity -= UpdateVisualBar;
    }

    private void UpdateVisualBar(int newActivityValue, DateTime? currentDateTime, bool isRestarting = false)
    {
        _currentActivityValue = Mathf.Clamp(_currentActivityValue + newActivityValue, 0, 100);
        float amount = _currentActivityValue / _maxActivity;

        float normalizedValue = Mathf.InverseLerp(_minActivity, _maxActivity, _currentActivityValue);
        Vector2 newPosition = Vector2.Lerp(_minPosition, _maxPosition, normalizedValue);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        _activityFillBar.color = _activityColorGradient.Evaluate(amount);
    }
}
