using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Master.Domain.Events;

public class UI_SettingsButton : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _closeSettingsButton;

    [Header("Volume")]
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

    [Header("Change Quetions")]
    [SerializeField] private GameObject _errorChangeQuestions_Window;
    [SerializeField] private GameObject _successChangeQuestions_Window;
    [SerializeField] private TMP_Text _errorChangeQuestions_TMP;
    [SerializeField] private TMP_InputField _inputChangeQuestiionsIF;
    private bool _isLoadingQuestions;

    [Header("Credits")]
    [SerializeField] private GameObject _credits_Section;
    
    private float _previousInitialHour;
    private float _currentInitialHour;
    private float _previousFinishHour;
    private float _currentFinishHour;

    private void Awake()
    {
        GameEvents_Questions.OnFinalizedCreationQuestions += ShowSuccessChangingQuestionsDB;
    }

    private void OnDestroy()
    {
        GameEvents_Questions.OnFinalizedCreationQuestions -= ShowSuccessChangingQuestionsDB;
    }

    void Start()
    {
        _warningWindow_Object.SetActive(false);
        _settingsPanel.SetActive(false);

        // Configuración inicial del volumen.
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

        //Configuración inicial Preguntas
        _successChangeQuestions_Window.SetActive(false);
        _isLoadingQuestions = false;

        //Configuración inicial Créditos
        CloseCredits();

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
        Animation_PageSliding.Instance.DeactivatePageSliding();
    }

    private void CloseSetting()
    {
        _settingsPanel?.SetActive(false);
        CancelChangeRangeTime();
        Animation_PageSliding.Instance.ActivatePageSliding();
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
        GameEvents_Graph.OnInitialTimeModified?.Invoke((int)_currentInitialHour);
        GameEvents_Graph.OnFinishTimeModified?.Invoke((int)_currentFinishHour);

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
        GameEvents_Graph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Activity);
        DataStorage.ResetHungerGraph();
        GameEvents_Graph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Hunger);
        DataStorage.ResetGlycemiaGraph();
        GameEvents_Graph.OnUpdatedAttributeGraph?.Invoke(GraphFilter.Glycemia);

        // Reset actions record
        DataStorage.ResetInsulinGraph();
        DataStorage.ResetFoodGraph();
        DataStorage.ResetExerciseGraph();
        GameEvents_Graph.OnUpdatedActionsGraph?.Invoke();

        // Reset actions CD and effects
        AttributeManager.Instance.DeactivateInsulinActionCD();
        AttributeManager.Instance.DeactivateInsulinEffect();
        AttributeManager.Instance.DeactivateExerciseActionCD();
        AttributeManager.Instance.DeactivateExerciseEffect();
        AttributeManager.Instance.DeactivateFoodActionCD();
        AttributeManager.Instance.DeactivateFoodEffect();
        GameEvents_PetCare.OnFinishTimerCD?.Invoke();

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

    public void OpenCredits()
    {
        _credits_Section.SetActive(true);
    }

    public void CloseCredits()
    {
        _credits_Section.SetActive(false);
    }

    // Se hacen las llamadas necesarias para saber si el archivo del enlace introducido es válido
    public void TryChangingQuestionsDB()
    {
        int result = DataStorage.SaveURLQuestions(_inputChangeQuestiionsIF.text);

        switch (result)
        {
            case 0:
                // Cargar las nuevas preguntas
                _isLoadingQuestions = true;
                _inputChangeQuestiionsIF.text = "";

                // Se reinician valores antes de buscar nuevas preguntas
                ResetQuestionsValues();
                // Se vuelven a buscar preguntas
                GameEvents_Questions.OnConfirmChangeQuestions?.Invoke();
                break;
            case -1:
                // Mostrar error URL vacío
                ShowErrorChangingQuestionsDB("No se ha escrito ningun enlace.");
                break;
            case -2:
                // Mostrar error de carga de archivo
                ShowErrorChangingQuestionsDB("Error al acceder al enlace.");
                break;
            case -3:
                // Mostrar error de formato de archivo
                ShowErrorChangingQuestionsDB("Error en el número de columnas del archivo.");
                break;
            case -4:
                // Mostrar error inesperado
                ShowErrorChangingQuestionsDB("Error inesperado.");
                break;
            case -5:
                // Mostrar error formato tsv
                ShowErrorChangingQuestionsDB("El formato del archivo no es \"tsv\".");
                break;
        }
    }

    private void ShowSuccessChangingQuestionsDB()
    {
        if (!_isLoadingQuestions)
            return;

        _isLoadingQuestions = false;
        _successChangeQuestions_Window.SetActive(true);
    }

    private void ShowErrorChangingQuestionsDB(string errorMsg)
    {
        _errorChangeQuestions_Window.SetActive(true);
        _errorChangeQuestions_TMP.text = errorMsg;
    }

    public void ResetQuestionsDB()
    {
        // Se reinician valores antes de buscar nuevas preguntas
        DataStorage.ResetQuestionURL();
        ResetQuestionsValues();

        // Se vuelven a buscar preguntas
        _isLoadingQuestions = true;
        GameEvents_Questions.OnConfirmChangeQuestions?.Invoke();
    }

    private void ResetQuestionsValues()
    {
        // Se reinicia el rendimiento en las preguntas, la iteración de preguntas y el indice de la pregunta.
        DataStorage.ResetUserPerformance();
        DataStorage.ResetIterationQuestions();
        DataStorage.ResetCurrentQuestionIndex();
    }
}
