using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.GameEvents;

namespace Master.Presentation.PetCare.Log
{
    public class UI_DateChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Text _date_TMP;
        [SerializeField] private Button _previousDate;
        [SerializeField] private Button _nextDate;

        private DateTime _currentDate = DateTime.Now.Date;

        void Start()
        {
            _date_TMP.text = $"{_currentDate.ToString("dd")} / {_currentDate.ToString("MM")} / {_currentDate.Year - 2000}";

            _previousDate.onClick.AddListener(PreviousDate);
            _nextDate.onClick.AddListener(NextDate);
        }

        private void UpdateDate()
        {
            _date_TMP.text = $"{_currentDate.ToString("dd")} / {_currentDate.ToString("MM")} / {_currentDate.Year - 2000}";
            GameEvents_PetCareLog.OnChangedDateFilter?.Invoke(_currentDate);
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
}