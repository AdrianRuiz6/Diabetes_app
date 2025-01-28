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
    private bool _isInRange;

    private float _time;
    private float _maxTime;
    private float _fillAmount;

    void Start()
    {
        _button = GetComponent<Button>();

        _isInCD = false;
        _backgroundImageCD.enabled = false;
        _iconImageCD.enabled = false;

        _isInRange = true;
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
        if (_isInRange == false && LimitHours.Instance.IsInRange(DateTime.Now.TimeOfDay))
        {
            EnableButton();
        }else if (_isInRange == true && LimitHours.Instance.IsInRange(DateTime.Now.TimeOfDay) == false)
        {
            DisableButton();
        }

        if (_isInRange)
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
    }

    private void StartTimerCD(string externalID, float timeCD)
    {
        if (externalID == _myID)
        {
            Debug.Log($"-test- timeCD = {timeCD}; externalID = {externalID}; gameObject = {gameObject.name}; Hora = {DateTime.Now.TimeOfDay}");
            _isInCD = true;
            _time = timeCD;
            _maxTime = AttributeManager.Instance.timeButtonsCD;

            _backgroundImageCD.enabled = true;
            _iconImageCD.enabled = true;
        }
    }

    public void DisableButton()
    {
        _isInRange = false;
        _button.interactable = false;

        _backgroundImageCD.enabled = true;
        _backgroundImageCD.fillAmount = 1;
        _iconImageCD.enabled = true;
        _iconImageCD.fillAmount = 1;
    }

    public void EnableButton()
    {
        _isInRange = true;
        _button.interactable = true;
        _time = 0;

        _backgroundImageCD.enabled = false; 
        _iconImageCD.enabled = false;
    }
}