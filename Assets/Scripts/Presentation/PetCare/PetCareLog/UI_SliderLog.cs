using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.GameEvents;
using Master.Domain.Settings;
using Master.Domain.PetCare.Log;
using Master.Infrastructure;
using TMPro;

namespace Master.Presentation.PetCare.Log
{
    public class UI_SliderLog : MonoBehaviour
    {
        private Slider _slider;

        [SerializeField] private TMP_Text _time_TMP;
        [SerializeField] private TMP_Text _insulinInfo_TMP;
        [SerializeField] private TMP_Text _exerciseInfo_TMP;
        [SerializeField] private TMP_Text _foodInfo_TMP;

        private int _minHour = 0;
        private int _maxHour = 0;

        private IPetCareLogManager _petCareLogManager;
        private ISettingsManager _settingsManager;

        private void Awake()
        {
            GameEvents_Settings.OnInitialTimeModified += ModifyInitialHour;
            GameEvents_Settings.OnFinishTimeModified += ModifyFinishHour;

            GameEvents_PetCareLog.OnUpdatedActionsLog += UpdateActionInfo;
            GameEvents_PetCareLog.OnChangedDateFilter += UpdateDateFilter;

            GameEvents_PetCareLog.OnResetSlider += UpdateDateFilter;
        }


#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Settings.OnInitialTimeModified -= ModifyInitialHour;
            GameEvents_Settings.OnFinishTimeModified -= ModifyFinishHour;

            GameEvents_PetCareLog.OnUpdatedActionsLog -= UpdateActionInfo;
            GameEvents_PetCareLog.OnChangedDateFilter -= UpdateDateFilter;

            GameEvents_PetCareLog.OnResetSlider -= UpdateDateFilter;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Settings.OnInitialTimeModified -= ModifyInitialHour;
                GameEvents_Settings.OnFinishTimeModified -= ModifyFinishHour;

                GameEvents_PetCareLog.OnUpdatedActionsLog -= UpdateActionInfo;
                GameEvents_PetCareLog.OnChangedDateFilter -= UpdateDateFilter;

                GameEvents_PetCareLog.OnResetSlider -= UpdateDateFilter;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Settings.OnInitialTimeModified -= ModifyInitialHour;
            GameEvents_Settings.OnFinishTimeModified -= ModifyFinishHour;

            GameEvents_PetCareLog.OnUpdatedActionsLog -= UpdateActionInfo;
            GameEvents_PetCareLog.OnChangedDateFilter -= UpdateDateFilter;

            GameEvents_PetCareLog.OnResetSlider -= UpdateDateFilter;
        }
#endif
        void Start()
        {
            _petCareLogManager = ServiceLocator.Instance.GetService<IPetCareLogManager>();
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();

            _slider = GetComponent<Slider>();
            _slider.wholeNumbers = true;

            // Se establece el minimo y máximo de la franja horaria.
            ModifyInitialHour(_settingsManager.initialTime.Hours);
            ModifyFinishHour(_settingsManager.finishTime.Hours);

            // Iniciar datos fecha actual.
            _slider.SetValueWithoutNotify(0);
            UpdateAdditionalInfo(0);

            _slider.onValueChanged.AddListener(ChangeValue);
        }

        private void UpdateDateFilter()
        {
            _slider.SetValueWithoutNotify(0);
            UpdateAdditionalInfo(0);
        }

        private void UpdateActionInfo()
        {
            UpdateAdditionalInfo((int)_slider.value);
        }

        // Se coloca en el valor disponible más cercano, escribe la fecha en el TMP correspondiente
        // y actualiza los datos de la información de los botones.
        private void ChangeValue(float value)
        {
            int closeValue = FindCloseValueTo((int)value);

            if (_slider.value != closeValue)
            {
                _slider.SetValueWithoutNotify(closeValue);
            }
            UpdateAdditionalInfo(closeValue);
        }

        // Ajusta el Slider al valor de _avalilableTimes de botón más cercano.
        private int FindCloseValueTo(int value)
        {
            List<DateTime> availableTimesThisDate = _petCareLogManager.GetActionsAvailableTimesThisDate(_petCareLogManager.currentDateFilter);
            if (availableTimesThisDate.Count <= 0)
            {
                return 0;
            }

            int closeValue = GetSliderValueAccordingTime(availableTimesThisDate[0]);
            int minimumDistance = Mathf.Abs(value - closeValue);

            foreach (DateTime newDate in availableTimesThisDate)
            {
                int currentValue = GetSliderValueAccordingTime(newDate);
                int currentDistance = Mathf.Abs(value - currentValue);

                if (currentDistance < minimumDistance)
                {
                    minimumDistance = currentDistance;
                    closeValue = currentValue;
                }
            }

            return closeValue;
        }

        // Escribe la fecha en el TMP correspondiente y actualiza los datos de la información de los botones.
        private void UpdateAdditionalInfo(int value)
        {
            DateTime currentTimeSlider = GetTimeAccordingSliderValue(value);
            _time_TMP.text = $"{currentTimeSlider.TimeOfDay}";

            bool isInsulinInfoFound = false;
            bool isExerciseInfoFound = false;
            bool isFoodInfoFound = false;

            foreach (ActionLog insulinLog in _petCareLogManager.insulinLogList)
            {
                if (currentTimeSlider.Hour == insulinLog.GetDateAndTime().Value.Hour && currentTimeSlider.Minute == insulinLog.GetDateAndTime().Value.Minute)
                {
                    _insulinInfo_TMP.text = insulinLog.GetInformation();
                    isInsulinInfoFound = true;
                }
            }
            if (isInsulinInfoFound == false)
            {
                _insulinInfo_TMP.text = "---";
            }

            foreach (ActionLog exerciseLog in _petCareLogManager.exerciseLogList)
            {
                if (currentTimeSlider.Hour == exerciseLog.GetDateAndTime().Value.Hour && currentTimeSlider.Minute == exerciseLog.GetDateAndTime().Value.Minute)
                {
                    _exerciseInfo_TMP.text = exerciseLog.GetInformation();
                    isExerciseInfoFound = true;
                }
            }
            if (isExerciseInfoFound == false)
            {
                _exerciseInfo_TMP.text = "---";
            }

            foreach (ActionLog foodLog in _petCareLogManager.foodLogList)
            {
                if (currentTimeSlider.Hour == foodLog.GetDateAndTime().Value.Hour && currentTimeSlider.Minute == foodLog.GetDateAndTime().Value.Minute)
                {
                    _foodInfo_TMP.text = foodLog.GetInformation();
                    isFoodInfoFound = true;
                }
            }
            if (isFoodInfoFound == false)
            {
                _foodInfo_TMP.text = "---";
            }
        }

        private void ModifyInitialHour(int hour)
        {
            _minHour = hour;
            _slider.minValue = 0;
            int maxHourAux = (_maxHour < _minHour) ? _maxHour + 24 : _maxHour;
            _slider.maxValue = (maxHourAux - _minHour) * 60 + 59;

            _slider.SetValueWithoutNotify(0);
            UpdateAdditionalInfo(0);
        }

        private void ModifyFinishHour(int hour)
        {
            _maxHour = hour;
            int maxHourAux = (_maxHour < _minHour) ? _maxHour + 24 : _maxHour;
            _slider.maxValue = (maxHourAux - _minHour) * 60 + 59;

            _slider.SetValueWithoutNotify(0);
            UpdateAdditionalInfo(0);
        }

        private int GetSliderValueAccordingTime(DateTime dateTime)
        {
            int dateTimeAux = (dateTime.Hour < _minHour) ? dateTime.Hour + 24 : dateTime.Hour;
            int sliderValue = (dateTimeAux - _minHour) * 60 + dateTime.Minute;
            return sliderValue;
        }

        private DateTime GetTimeAccordingSliderValue(int sliderValue)
        {
            DateTime additionalTime = new DateTime(_petCareLogManager.currentDateFilter.Year, _petCareLogManager.currentDateFilter.Month, _petCareLogManager.currentDateFilter.Day, sliderValue / 60, sliderValue % 60, 0);
            DateTime minimumTime = new DateTime(_petCareLogManager.currentDateFilter.Year, _petCareLogManager.currentDateFilter.Month, _petCareLogManager.currentDateFilter.Day, _minHour, 0, 0);
            DateTime time = new DateTime(_petCareLogManager.currentDateFilter.Year, _petCareLogManager.currentDateFilter.Month, _petCareLogManager.currentDateFilter.Day, minimumTime.Hour + additionalTime.Hour, additionalTime.Minute, 0);
            return time;
        }
    }
}