using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Master.Presentation.Animations;
using Master.Presentation.PetCare.Log;
using Master.Presentation.Sound;
using Master.Domain.GameEvents;
using Master.Domain.Score;
using Master.Domain.PetCare;
using Master.Persistence;
using Master.Persistence.Settings;
using Master.Persistence.PetCare;
using Master.Persistence.Questions;
using Master.Domain.Settings;

namespace Master.Presentation.Settings
{
    public class UI_SettingsButton : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _closeSettingsButton;

        [Header("Sound")]
        [SerializeField] private Slider _sliderSoundEffects;

        [Header("Range hours")]
        [SerializeField] private Slider _sliderInitialHour;
        [SerializeField] private TMP_Text _initialHour_TMP;
        [SerializeField] private Slider _sliderFinishHour;
        [SerializeField] private TMP_Text _finishHour_TMP;
        [SerializeField] private GameObject _warningWindow_Object;
        [SerializeField] private Button _applySettingsButton;
        [SerializeField] private Button _closeWarningButton;
        [SerializeField] private Button _confirmWarningButton;

        [Header("Questions change")]
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
            GameEvents_Settings.OnSoundEffectsInitialized += InitializeSoundEffect;

            GameEvents_Settings.OnInitialTimeInitialized += InititializeInitialHour;
            GameEvents_Settings.OnFinishTimeInitialized += InitializeFinishHour;

            GameEvents_Questions.OnFinalizedCreationQuestions += ShowSuccessChangingQuestionsDB;
        }

        private void OnDestroy()
        {
            GameEvents_Settings.OnSoundEffectsInitialized -= InitializeSoundEffect;

            GameEvents_Settings.OnInitialTimeInitialized -= InititializeInitialHour;
            GameEvents_Settings.OnFinishTimeInitialized -= InitializeFinishHour;

            GameEvents_Questions.OnFinalizedCreationQuestions -= ShowSuccessChangingQuestionsDB;
        }

        void Start()
        {
            // Configuración general.
            _warningWindow_Object.SetActive(false);
            _settingsPanel.SetActive(false);
            
            _openSettingsButton.onClick.AddListener(OpenSetting);
            _closeSettingsButton.onClick.AddListener(CloseSetting);
            _applySettingsButton.onClick.AddListener(ShowWarningChangeRangeTime);
            _closeWarningButton.onClick.AddListener(CloseWarningChangeRangeTime);
            _confirmWarningButton.onClick.AddListener(ConfirmChangeRangeTime);

            // Configuración inicial del sonido.
            _sliderSoundEffects.wholeNumbers = false;
            _sliderSoundEffects.minValue = 0;
            _sliderSoundEffects.maxValue = 1;
            _sliderSoundEffects.value = 0;
            _sliderSoundEffects.onValueChanged.AddListener(ChangeSoundEffectsVolume);

            //Configuración inicial de la franja horaria.
            _sliderInitialHour.wholeNumbers = true;
            _sliderInitialHour.minValue = 0;
            _sliderInitialHour.maxValue = 23;
            _sliderInitialHour.value =0;
            SetInitialHourTMP(0);
            _sliderInitialHour.onValueChanged.AddListener(ChangeInitialHour);

            _sliderFinishHour.wholeNumbers = true;
            _sliderFinishHour.minValue = 0;
            _sliderFinishHour.maxValue = 23;
            _sliderFinishHour.value = 0;
            SetFinishHourTMP(0);
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
        }

        void Update()
        {
            if (_previousInitialHour != _currentInitialHour || _previousFinishHour != _currentFinishHour)
            {
                _applySettingsButton.gameObject.SetActive(true);
            }
            else
            {
                if (_applySettingsButton.gameObject.activeSelf)
                    _applySettingsButton.gameObject.SetActive(false);
            }
        }

        #region Navigation
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
        #endregion

        #region Sound
        private void InitializeSoundEffect(float value)
        {
            _sliderSoundEffects.value = value;
        }

        private void ChangeSoundEffectsVolume(float value)
        {
            SettingsManager.SetSoundEffectsVolume(value);
        }
        #endregion

        #region Range hours change
        private void InititializeInitialHour(int hour)
        {
            _sliderInitialHour.value = hour;
            SetInitialHourTMP(hour);

            _previousInitialHour = _sliderInitialHour.value;
            _currentInitialHour = _sliderInitialHour.value;
        }

        private void InitializeFinishHour(int hour)
        {
            _sliderFinishHour.value = hour;
            SetFinishHourTMP(hour);

            _previousFinishHour = _sliderFinishHour.value;
            _currentFinishHour = _sliderFinishHour.value;
        }

        private void ChangeInitialHour(float hour)
        {
            if (hour > _sliderFinishHour.value)
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
            SettingsManager.ConfirmChangeRangeTime();

            _previousInitialHour = _currentInitialHour;
            _previousFinishHour = _currentFinishHour;

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
        #endregion

        #region Questions change
        // Se hacen las llamadas necesarias para saber si el archivo del enlace introducido es válido
        public void TryChangingQuestionsDB()
        {
            int result = DataStorage_Questions.SaveURLQuestions(_inputChangeQuestiionsIF.text);

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
            DataStorage_Questions.ResetQuestionURL();
            ResetQuestionsValues();

            // Se vuelven a buscar preguntas
            _isLoadingQuestions = true;
            GameEvents_Questions.OnConfirmChangeQuestions?.Invoke();
        }

        private void ResetQuestionsValues()
        {
            // Se reinicia el rendimiento en las preguntas, la iteración de preguntas y el indice de la pregunta.
            DataStorage_Questions.ResetUserPerformance();
            DataStorage_Questions.ResetIterationQuestions();
            DataStorage_Questions.SaveCurrentQuestionIndex(0);
        }
        #endregion

        #region Credits
        public void OpenCredits()
        {
            _credits_Section.SetActive(true);
        }

        public void CloseCredits()
        {
            _credits_Section.SetActive(false);
        }
        #endregion
    }
}