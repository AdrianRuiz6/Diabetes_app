using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SliderGraph : MonoBehaviour
{
    private Slider _slider;

    [SerializeField] private TMP_Text _time_TMP;
    [SerializeField] private TMP_Text _insulinInfo_TMP;
    [SerializeField] private TMP_Text _exerciseInfo_TMP;
    [SerializeField] private TMP_Text _foodInfo_TMP;

    private int _minHour = 0;
    private int _maxHour = 0;

    private List<DateTime> _availableTimes = new List<DateTime>();
    private Dictionary<DateTime, string> _insulinInfo = new Dictionary<DateTime, string>();
    private Dictionary<DateTime, string> _exerciseInfo = new Dictionary<DateTime, string>();
    private Dictionary<DateTime, string> _foodInfo = new Dictionary<DateTime, string>();
    private DateTime _currentDate = DateTime.Now;

    private void Awake()
    {
        GameEventsGraph.OnInitialTimeModified += ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified += ModifyFinishHour;

        GameEventsGraph.OnUpdatedActionsGraph += LoadData;

        GameEventsGraph.OnUpdatedDateGraph += UpdateDate;
    }

    private void OnDestroy()
    {
        GameEventsGraph.OnInitialTimeModified -= ModifyInitialHour;
        GameEventsGraph.OnFinishTimeModified -= ModifyFinishHour;

        GameEventsGraph.OnUpdatedActionsGraph -= LoadData;

        GameEventsGraph.OnUpdatedDateGraph -= UpdateDate;
    }

    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.wholeNumbers = true;

        // Se establece el minimo y máximo de la franja horaria.
        ModifyInitialHour(LimitHours.Instance.initialTime.Hours);
        ModifyFinishHour(LimitHours.Instance.finishTime.Hours);

        // Iniciar datos fecha actual.
        UpdateDate(DateTime.Now);

        _slider.onValueChanged.AddListener(ChangeValue);
    }

    private void UpdateDate(DateTime newCurrentDate)
    {
        _currentDate = newCurrentDate;
        LoadData();

        _slider.SetValueWithoutNotify(0);
        UpdateAdditionalInfo(0);
    }

    // Se coloca en el valor disponible más cercano, escribe la fecha en el TMP correspondiente
    // y actualiza los datos de la información de los botones.
    private void ChangeValue(float value)
    {
        int closeValue = FindCloseValueTo((int)value);

        if(_slider.value != closeValue)
        {
            _slider.SetValueWithoutNotify(closeValue);
        }
        UpdateAdditionalInfo(closeValue);
    }

    // Ajusta el Slider al valor de _avalilableTimes de botón más cercano.
    private int FindCloseValueTo(int value)
    {
        if(_availableTimes.Count <= 0)
        {
            return 0;
        }

        int closeValue = GetSliderValueAccordingTime(_availableTimes[0]);
        int minimumDistance = Mathf.Abs(value - closeValue);

        foreach(DateTime newDate in _availableTimes)
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

        foreach(KeyValuePair<DateTime, String> kvp in _insulinInfo)
        {
            if(currentTimeSlider.Hour == kvp.Key.Hour && currentTimeSlider.Minute == kvp.Key.Minute)
            {
                _insulinInfo_TMP.text = kvp.Value.ToString();
                isInsulinInfoFound = true;
            }
        }
        if(isInsulinInfoFound == false)
        {
            _insulinInfo_TMP.text = "---";
        }

        foreach (KeyValuePair<DateTime, String> kvp in _exerciseInfo)
        {
            if (currentTimeSlider.Hour == kvp.Key.Hour && currentTimeSlider.Minute == kvp.Key.Minute)
            {
                _exerciseInfo_TMP.text = kvp.Value.ToString();
                isExerciseInfoFound = true;
            }
        }
        if (isExerciseInfoFound == false)
        {
            _exerciseInfo_TMP.text = "---";
        }

        foreach (KeyValuePair<DateTime, String> kvp in _foodInfo)
        {
            if (currentTimeSlider.Hour == kvp.Key.Hour && currentTimeSlider.Minute == kvp.Key.Minute)
            {
                _foodInfo_TMP.text = kvp.Value.ToString();
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

        // TODO
        
    }

    private void ModifyFinishHour(int hour)
    {
        _maxHour = hour;
        int maxHourAux = 0;
        if(_maxHour < _minHour)
        {
            maxHourAux = _maxHour + 24;
        }
        else
        {
            maxHourAux = _maxHour;
        }
        _slider.maxValue = (maxHourAux - _minHour) * 60 + 59;

        // TODO

    }

    private int GetSliderValueAccordingTime(DateTime dateTime)
    {
        int dateTimeAux = 0;
        if (dateTime.Hour < _minHour)
        {
            dateTimeAux = dateTime.Hour + 24;
        }
        else
        {
            dateTimeAux = dateTime.Hour;
        }
        int sliderValue = (dateTimeAux - _minHour) * 60 + dateTime.Minute;
        return sliderValue;
    }

    private DateTime GetTimeAccordingSliderValue(int sliderValue)
    {
        DateTime additionalTime = new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, sliderValue / 60, sliderValue % 60, 0);
        DateTime minimumTime = new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, _minHour, 0, 0);
        DateTime time = new DateTime(_currentDate.Year, _currentDate.Month, _currentDate.Day, minimumTime.Hour + additionalTime.Hour, additionalTime.Minute, 0);
        return time;
    }

    // Carga los datos de los botones y llena la lista de availableDates
    private void LoadData()
    {
        _insulinInfo.Clear();
        _exerciseInfo.Clear();
        _foodInfo.Clear();
        _availableTimes.Clear();

        _insulinInfo = DataStorage.LoadInsulinGraph(_currentDate);
        foreach (DateTime newDate in _insulinInfo.Keys)
        {
            if (!_availableTimes.Contains(newDate))
            {
                _availableTimes.Add(newDate);
            }
        }
        _exerciseInfo = DataStorage.LoadExerciseGraph(_currentDate);
        foreach (DateTime newDate in _exerciseInfo.Keys)
        {
            if (!_availableTimes.Contains(newDate))
            {
                _availableTimes.Add(newDate);
            }
        }
        _foodInfo = DataStorage.LoadFoodGraph(_currentDate);
        foreach (DateTime newDate in _foodInfo.Keys)
        {
            if (!_availableTimes.Contains(newDate))
            {
                _availableTimes.Add(newDate);
            }
        }
    }
}
