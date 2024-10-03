using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_DateChanger : MonoBehaviour
{
    [SerializeField] private TMP_Text _date_TMP;
    [SerializeField] private Button _previousDate;
    [SerializeField] private Button _nextDate;

    private DateTime _currentDate = DateTime.Now.Date;

    void Start()
    {
        UpdateDate();

        _previousDate.onClick.AddListener(PreviousDate);
        _nextDate.onClick.AddListener(NextDate);
    }

    private void UpdateDate()
    {
        _date_TMP.text = $"{_currentDate.ToString("dd")} / {_currentDate.ToString("MM")} / {_currentDate.Year - 2000}";
        GameEventsGraph.OnUpdatedDateGraph?.Invoke(_currentDate);
    }

    private void PreviousDate()
    {
        _currentDate = _currentDate.AddDays(-1);
        UpdateDate();
    }

    private void NextDate()
    {
        if (_currentDate.AddDays(1) <= DateTime.Now.Date)
        {
            _currentDate = _currentDate.AddDays(1);
            UpdateDate();
        }
    }
}
