using Master.Domain.GameEvents;
using System;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.Settings;
using Master.Domain.PetCare;
using Unity.VisualScripting;

namespace Master.Presentation.PetCare
{
    public class Cooldown_Actions : MonoBehaviour
    {
        [SerializeField] private ActionType _actionType;
        [SerializeField] private Image _backgroundImageCD;
        [SerializeField] private Image _iconImageCD;

        private Button _button;
        private bool _isInCD;
        private bool _isInRange;

        private float _time;
        private float _maxTime;
        private float _fillAmount;

        private IPetCareManager _petCareManager;
        private ISettingsManager _settingsManager;

        private void Awake()
        {
            GameEvents_PetCare.OnStartTimerCD += StartTimerCD;
            GameEvents_PetCare.OnFinishTimerCD += FinishCD;
            GameEvents_Settings.OnInitialTimeModified += CheckEnabled;
            GameEvents_Settings.OnFinishTimeModified += CheckEnabled;
        }

        void OnDestroy()
        {
            GameEvents_PetCare.OnStartTimerCD -= StartTimerCD;
            GameEvents_PetCare.OnFinishTimerCD -= FinishCD;
            GameEvents_Settings.OnInitialTimeModified -= CheckEnabled;
            GameEvents_Settings.OnFinishTimeModified -= CheckEnabled;
        }

        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();

            _button = GetComponent<Button>();

            _isInCD = false;
            _backgroundImageCD.enabled = false;
            _iconImageCD.enabled = false;

            CheckEnabled();

            switch (_actionType)
            {
                case ActionType.Insulin:
                    if(_petCareManager.isInsulinActionInCD)
                        StartTimerCD(_actionType, _petCareManager.currentTimeCDInsulin);
                    break;
                case ActionType.Food:
                    if (_petCareManager.isFoodActionInCD)
                        StartTimerCD(_actionType, _petCareManager.currentTimeCDFood);
                    break;
                case ActionType.Exercise:
                    if (_petCareManager.isExerciseActionInCD)
                        StartTimerCD(_actionType, _petCareManager.currentTimeCDExercise);
                    break;
            }
        }

        void Update()
        {
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
                    FinishCD(_actionType);
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

        private void StartTimerCD(ActionType requestedType, float timeCD)
        {
            if (requestedType == _actionType)
            {
                Debug.Log($"-test- timeCD = {timeCD}; externalID = {requestedType}; gameObject = {gameObject.name}; Hora = {DateTime.Now.TimeOfDay}");
                _isInCD = true;
                _time = timeCD;
                _maxTime = _petCareManager.timeCDActions;

                _backgroundImageCD.enabled = true;
                _iconImageCD.enabled = true;
            }
        }

        private void FinishCD(ActionType requestedType)
        {
            if (requestedType == _actionType)
            {
                _time = 0;
                _isInCD = false;
                _backgroundImageCD.enabled = false;
                _iconImageCD.enabled = false;
                _button.interactable = true;
            }
        }

        private void CheckEnabled(int hour = 0)
        {
            if (_settingsManager.IsInRange(DateTime.Now.TimeOfDay))
            {
                EnableButton();
            }
            else
            {
                DisableButton();
            }
        }

        private void DisableButton()
        {
            _isInRange = false;
            _button.interactable = false;

            _backgroundImageCD.enabled = true;
            _backgroundImageCD.fillAmount = 1;
            _iconImageCD.enabled = true;
            _iconImageCD.fillAmount = 1;
        }

        private void EnableButton()
        {
            _isInRange = true;
            _button.interactable = true;
            _time = 0;

            _backgroundImageCD.enabled = false;
            _iconImageCD.enabled = false;
        }
    }
}