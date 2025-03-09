using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Master.Domain.Events;

public class SettingsButton : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _closeSettingsButton;

    [Header("Volume")]
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSoundEffects;

    [Header("Limit hours")]
    [SerializeField] private Slider _sliderInitialHour;
    [SerializeField] private TMP_Text _initialHour_TMP;
    [SerializeField] private Slider _sliderFinishHour;
    [SerializeField] private TMP_Text _finishHour_TMP;
    [SerializeField] private GameObject _warningWindow_Object;
    [SerializeField] private Button _applySettingsButton;
    [SerializeField] private Button _closeWarningButton;
    [SerializeField] private Button _confirmWarningButton;

    private float _previousInitialHour;
    private float _currentInitialHour;
    private float _previousFinishHour;
    private float _currentFinishHour;

    void Start()
    {
        _warningWindow_Object.SetActive(false);
        _settingsPanel.SetActive(false);

        // Configuración inicial del volumen.
        _sliderMusic.wholeNumbers = false;
        _sliderMusic.minValue = 0;
        _sliderMusic.maxValue = 1;
        _sliderMusic.value = DataStorage.LoadMusicVolume();
        _sliderMusic.onValueChanged.AddListener(ChangeMusicVolume);

        _sliderSoundEffects.wholeNumbers = false;
        _sliderSoundEffects.minValue = 0;
        _sliderSoundEffects.maxValue = 1;
        _sliderSoundEffects.value = DataStorage.LoadSoundEffectsVolume(); ;
        _sliderSoundEffects.onValueChanged.AddListener(ChangeSoundEffectsVolume);

        //Configuración inicial de la franja horaria.
        _sliderInitialHour.wholeNumbers = true;
        _sliderInitialHour.minValue = 0;
        _sliderInitialHour.maxValue = 23;
        _sliderInitialHour.value = DataStorage.LoadInitialTime().Hours;
        SetInitialHourTMP(DataStorage.LoadInitialTime().Hours);
        _sliderInitialHour.onValueChanged.AddListener(ChangeInitialHour);

        _sliderFinishHour.wholeNumbers = true;
        _sliderFinishHour.minValue = 0;
        _sliderFinishHour.maxValue = 23;
        _sliderFinishHour.value = DataStorage.LoadFinishTime().Hours;
        SetFinishHourTMP(DataStorage.LoadFinishTime().Hours);
        _sliderFinishHour.onValueChanged.AddListener(ChangeFinishHour);

        _previousInitialHour = _sliderInitialHour.value;
        _currentInitialHour = _sliderInitialHour.value;
        _previousFinishHour = _sliderFinishHour.value;
        _currentFinishHour = _sliderFinishHour.value;

    // Configuración de los botones.
        _openSettingsButton.onClick.AddListener(OpenSetting);
        _closeSettingsButton.onClick.AddListener(CloseSetting);
        _applySettingsButton.onClick.AddListener(ShowWarningChangeRangeTime);
        _closeWarningButton.onClick.AddListener(CloseWarningChangeRangeTime);
        _confirmWarningButton.onClick.AddListener(ConfirmChangeRangeTime);
    }

    void Update()
    {
        if(_previousInitialHour != _currentInitialHour || _previousFinishHour != _currentFinishHour){
            _applySettingsButton.gameObject.SetActive(true);
        }else
        {
            if (_applySettingsButton.gameObject.activeSelf)
                _applySettingsButton.gameObject.SetActive(false);
        }
    }

    private void OpenSetting()
    {
        _settingsPanel.SetActive(true);
        PageSliding.Instance.DeactivatePageSliding();
    }

    private void CloseSetting()
    {
        _settingsPanel?.SetActive(false);
        CancelChangeRangeTime();
        PageSliding.Instance.ActivatePageSliding();
    }

    private void ChangeMusicVolume(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);
    }

    private void ChangeSoundEffectsVolume(float value)
    {
        SoundManager.Instance.SetSoundEffectsVolume(value);
    }

    private void ChangeInitialHour(float hour)
    {
        if(hour > _sliderFinishHour.value)
        {
            hour = _sliderFinishHour.value;
            _sliderInitialHour.SetValueWithoutNotify(hour);
        }

        _currentInitialHour = _sliderInitialHour.value;
        SetInitialHourTMP(hour);
    }

    private void SetInitialHourTMP(float hour)
    {
        string hourText = (hour < 10) ? $"0{hour}" : hour.ToString();
        _initialHour_TMP.text = $"{hourText}:00";
    }

    private void ChangeFinishHour(float hour)
    {
        if (hour < _sliderInitialHour.value)
        {
            hour = _sliderInitialHour.value;
            _sliderFinishHour.SetValueWithoutNotify(hour);
        }

        _currentFinishHour = _sliderFinishHour.value;
        SetFinishHourTMP(hour);
    }

    private void SetFinishHourTMP(float hour)
    {
        string hourText = (hour < 10) ? $"0{hour}" : hour.ToString();
        _finishHour_TMP.text = $"{hourText}:59";
    }

    private void ShowWarningChangeRangeTime()
    {
        _warningWindow_Object.SetActive(true);
    }

    private void CloseWarningChangeRangeTime()
    {
        _warningWindow_Object.SetActive(false);
    }

    private void ConfirmChangeRangeTime()
    {
        GameEventsGraph.OnInitialTimeModified?.Invoke((int)_currentInitialHour);
        GameEventsGraph.OnFinishTimeModified?.Invoke((int)_currentFinishHour);

        _previousInitialHour = _currentInitialHour;
        _previousFinishHour = _currentFinishHour;

        // Reset score
        ScoreManager.Instance.ResetScore();

        // Reset attributes
        AttributeManager.Instance.RestartGlycemia(DateTime.Now);
        AttributeManager.Instance.RestartActivity(DateTime.Now);
        AttributeManager.Instance.RestartHunger(DateTime.Now);

        // Reset attributes record
        DataStorage.ResetActivityGraph();
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Activity);
        DataStorage.ResetHungerGraph();
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Hunger);
        DataStorage.ResetGlycemiaGraph();
        GameEventsGraph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Glycemia);

        // Reset actions record
        DataStorage.ResetInsulinGraph();
        DataStorage.ResetFoodGraph();
        DataStorage.ResetExerciseGraph();
        GameEventsGraph.OnUpdatedActionsGraph?.Invoke();

        // Reset actions CD and effects
        AttributeManager.Instance.DeactivateInsulinButtonCD();
        AttributeManager.Instance.DeactivateInsulinEffect();
        AttributeManager.Instance.DeactivateExerciseButtonCD();
        AttributeManager.Instance.DeactivateExerciseEffect();
        AttributeManager.Instance.DeactivateFoodButtonCD();
        AttributeManager.Instance.DeactivateFoodEffect();
        GameEventsPetCare.OnFinishTimerCD?.Invoke();

        CloseWarningChangeRangeTime();
    }

    private void CancelChangeRangeTime()
    {
        _sliderInitialHour.SetValueWithoutNotify(_previousInitialHour);
        SetInitialHourTMP(_previousInitialHour);

        _sliderFinishHour.SetValueWithoutNotify(_previousFinishHour);
        SetFinishHourTMP(_previousFinishHour);

        _currentInitialHour = _previousInitialHour;
        _currentFinishHour = _previousFinishHour;
    }
}
