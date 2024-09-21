using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameEventsPetCare.OnModifyHunger += UpdateVisualBar;
    }

    void OnDestroy()
    {
        GameEventsPetCare.OnModifyHunger -= UpdateVisualBar;
    }

    private void UpdateVisualBar(int newHungerValue)
    {
        _currentHungerValue = Mathf.Clamp(_currentHungerValue + newHungerValue, 0, 100);
        float amount = _currentHungerValue / _maxHunger;

        float normalizedValue = Mathf.InverseLerp(_minHunger, _maxHunger, _currentHungerValue);
        Vector2 newPosition = Vector2.Lerp(_minPosition, _maxPosition, normalizedValue);
        _valueObject.transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        _hungerFillBar.color = _hungerColorGradient.Evaluate(amount);
    }
}
