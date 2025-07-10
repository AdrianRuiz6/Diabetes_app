using Master.Domain.GameEvents;
using System;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.Settings;
using Master.Domain.PetCare;
using Master.Infrastructure;

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

        private float _maxTime;
        private float _fillAmount;

        private IPetCareManager _petCareManager;
        private ISettingsManager _settingsManager;

        private DateTime _cooldownEndTime;
        private void Awake()
        {
            GameEvents_PetCare.OnStartTimerCD += StartCD;
            GameEvents_PetCare.OnFinishTimerCD += FinishCD;
            GameEvents_Settings.OnInitialTimeModified += CheckEnabled;
            GameEvents_Settings.OnFinishTimeModified += CheckEnabled;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            GameEvents_PetCare.OnStartTimerCD -= StartCD;
            GameEvents_PetCare.OnFinishTimerCD -= FinishCD;
            GameEvents_Settings.OnInitialTimeModified -= CheckEnabled;
            GameEvents_Settings.OnFinishTimeModified -= CheckEnabled;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                GameEvents_PetCare.OnStartTimerCD -= StartCD;
                GameEvents_PetCare.OnFinishTimerCD -= FinishCD;
                GameEvents_Settings.OnInitialTimeModified -= CheckEnabled;
                GameEvents_Settings.OnFinishTimeModified -= CheckEnabled;
            }
        }

        void OnApplicationQuit()
        {
            GameEvents_PetCare.OnStartTimerCD -= StartCD;
            GameEvents_PetCare.OnFinishTimerCD -= FinishCD;
            GameEvents_Settings.OnInitialTimeModified -= CheckEnabled;
            GameEvents_Settings.OnFinishTimeModified -= CheckEnabled;
        }
#endif

        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
            _settingsManager = ServiceLocator.Instance.GetService<ISettingsManager>();

            _button = GetComponent<Button>();

            _maxTime = _petCareManager.timeCDActions;

            _isInCD = false;
            _backgroundImageCD.enabled = false;
            _iconImageCD.enabled = false;
            _isInRange = true;

            CheckEnabled();
            StartCD(_actionType);
        }

        void Update()
        {
            CheckEnabled();

            if (_isInCD)
            {
                float remainingSeconds = Mathf.Max(0f, (float)(_cooldownEndTime - DateTime.Now).TotalSeconds);

                if (remainingSeconds <= 0)
                {
                    FinishCD(_actionType);
                }
                else
                {
                    _fillAmount = Mathf.Clamp01(remainingSeconds / _maxTime);
                    _backgroundImageCD.fillAmount = _fillAmount;
                    _iconImageCD.fillAmount = _fillAmount;
                }

                _button.interactable = false;
            }
            else
            {
                _button.interactable = true;
            }
        }

        private void StartCD(ActionType actionType)
        {
            if (_actionType == actionType)
            {
                switch (_actionType)
                {
                    case ActionType.Insulin:
                        _isInCD = true;
                        _backgroundImageCD.enabled = true;
                        _iconImageCD.enabled = true;
                        _cooldownEndTime = _petCareManager.insulinCooldownEndTime;
                        break;
                    case ActionType.Food:
                        _isInCD = true;
                        _backgroundImageCD.enabled = true;
                        _iconImageCD.enabled = true;
                        _cooldownEndTime = _petCareManager.foodCooldownEndTime;
                        break;
                    case ActionType.Exercise:
                        _isInCD = true;
                        _backgroundImageCD.enabled = true;
                        _iconImageCD.enabled = true;
                        _cooldownEndTime = _petCareManager.exerciseCooldownEndTime;
                        break;
                }
            }
        }

        private void FinishCD(ActionType requestedType)
        {
            if (requestedType == _actionType)
            {
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
                if (!_isInRange)
                {
                    EnableButton();
                    _isInRange = true;
                }
            }
            else
            {
                DisableButton();
                _isInRange = false;
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

            _backgroundImageCD.enabled = false;
            _iconImageCD.enabled = false;
        }
    }
}