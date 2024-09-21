using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    [SerializeField] private string _myID;
    [SerializeField] private Image _backgroundImageCD;
    [SerializeField] private Image _iconImageCD;

    private Button _button;
    private bool _isInCD;

    private float _time;
    private float _maxTime;
    private float _fillAmount;

    void Start()
    {
        _button = GetComponent<Button>();

        _maxTime = AttributeManager.Instance.timeButtonsCD;
        _isInCD = false;
        _backgroundImageCD.enabled = false;
        _iconImageCD.enabled = false;
    }

    private void Awake()
    {
        GameEventsPetCare.OnStartTimerCD += StartTimerCD;
    }

    void OnDestroy()
    {
        GameEventsPetCare.OnStartTimerCD -= StartTimerCD;
    }

    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;

            // Visualilzación radial del CoolDown.
            _fillAmount = _time / _maxTime;
            _fillAmount = Mathf.Clamp01(_fillAmount);

            _backgroundImageCD.fillAmount = _fillAmount;
            _iconImageCD.fillAmount = _fillAmount;
        }

        if (_time <= 0)
        {
            _isInCD = false;
            _backgroundImageCD.enabled = false;
            _iconImageCD.enabled = false;
        }

        if (_isInCD)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }

    private void StartTimerCD(string externalID, float timeCD)
    {
        if (externalID == _myID)
        {
            _isInCD = true;
            _time = timeCD;

            _backgroundImageCD.enabled = true;
            _iconImageCD.enabled = true;
        }
    }
}