using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Master.Presentation.Animations;
using Master.Domain.GameEvents;
using Master.Domain.Settings;
using Master.Infrastructure;

namespace Master.Presentation.Settings
{
    public class UI_Settings : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _closeSettingsButton;
        private bool _currentPanelActive;
        private bool _previousPanelActive;

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

        private ISettingsManager _settingsManager;

        private void Awake()
        {
            GameEvents_Questions.OnFinishQuestionSearch += ShowSuccessChangingQuestionsDB;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        private void OnDestroy()
        {
            GameEvents_Questions.OnFinishQuestionSearch -= ShowSuccessChangingQuestionsDB;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_Questions.OnFinishQuestionSearch -= ShowSuccessChangingQuestionsDB;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_Questions.OnFinishQuestionSearch -= ShowSuccessChangingQuestionsDB;
        }
#endif

        void Start()
        {
            _previousPanelActive = false;
            _currentPanelActive = false;
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();

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
            InitializeSoundEffect(_settingsManager.soundEffectsVolume);

            //Configuración inicial de la franja horaria.
            _sliderInitialHour.wholeNumbers = true;
            _sliderInitialHour.minValue = 0;
            _sliderInitialHour.maxValue = 23;
            SetInitialHourTMP(0);
            InititializeInitialHour(_settingsManager.initialTime.Hours);
            _sliderInitialHour.onValueChanged.AddListener(ChangeInitialHour);

            _sliderFinishHour.wholeNumbers = true;
            _sliderFinishHour.minValue = 0;
            _sliderFinishHour.maxValue = 23;
            SetFinishHourTMP(0);
            InitializeFinishHour(_settingsManager.finishTime.Hours);
            _sliderFinishHour.onValueChanged.AddListener(ChangeFinishHour);

            

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

            CheckPageSliding();
        }

        private void CheckPageSliding()
        {
            if (_currentPanelActive == false && _previousPanelActive == true)
            {
                _previousPanelActive = false;
                Animation_PageSliding.Instance.ActivatePageSliding();
            }else if (_currentPanelActive == true && _previousPanelActive == false)
            {
                _previousPanelActive = true;
                Animation_PageSliding.Instance.DeactivatePageSliding();
            }
        }

        #region Navigation
        private void OpenSetting()
        {
            _settingsPanel.SetActive(true);
            _previousPanelActive = _currentPanelActive;
            _currentPanelActive = true;
        }

        private void CloseSetting()
        {
            _settingsPanel?.SetActive(false);
            CancelChangeRangeTime();
            _previousPanelActive = _currentPanelActive;
            _currentPanelActive = false;
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        #endregion

        #region Sound
        private void InitializeSoundEffect(float value)
        {
            _sliderSoundEffects.value = value;
        }

        private void ChangeSoundEffectsVolume(float value)
        {
            _settingsManager.SetSoundEffectsVolume(value);
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
            _settingsManager.ConfirmChangeRangeTime(_currentInitialHour, _currentFinishHour);

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
        public void OnChangeQuestions()
        {
            int result = _settingsManager.TryChangingQuestionsURL(_inputChangeQuestiionsIF.text);

            switch (result)
            {
                case 0:
                    // Cargar las nuevas preguntas
                    _inputChangeQuestiionsIF.text = "";
                    _isLoadingQuestions = true;
                    bool isOK = _settingsManager.ChangeQuestions();
                    if (isOK == false)
                    {
                        _isLoadingQuestions = false;
                        ShowErrorChangingQuestionsDB("Error cargando preguntas.");
                    }
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
            if (_isLoadingQuestions)
            {
                _isLoadingQuestions = false;
                _successChangeQuestions_Window.SetActive(true);
            }
        }

        private void ShowErrorChangingQuestionsDB(string errorMsg)
        {
            _errorChangeQuestions_Window.SetActive(true);
            _errorChangeQuestions_TMP.text = errorMsg;
        }

        public void OnResetQuestionsDB()
        {
            _isLoadingQuestions = true;
            bool isOK = _settingsManager.ResetQuestions();
            if (isOK == false)
            {
                _isLoadingQuestions = false;
                ShowErrorChangingQuestionsDB("Error cargando preguntas.");
            }
            else
            {
                ShowSuccessChangingQuestionsDB();
            }
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