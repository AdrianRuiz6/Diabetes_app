using Master.Domain.GameEvents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Master.Domain.PetCare;
using Master.Infrastructure;
using Master.Domain.Score;

namespace Master.Presentation.PetCare
{
    public class UI_Attribute : MonoBehaviour
    {
        [SerializeField] private AttributeType _attributeType;

        private float _minValue;
        private float _maxValue;
        private float _currentValue;

        private List<AttributeState> _attributeStates;
        private Slider _slider;
        private Image _sliderBackground;
        private TextMeshProUGUI _value_TXT;
        private string _unit;

        [SerializeField] private Color _colorGood;
        [SerializeField] private Color _colorIntermediate;
        [SerializeField] private Color _colorBad;

        private IPetCareManager _petCareManager;

        private bool _isSimulationFinished = false;

        void Awake()
        {
            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    GameEvents_PetCare.OnModifyGlycemiaUI += UpdateVisualBar;
                    break;
                case AttributeType.Energy:
                    GameEvents_PetCare.OnModifyEnergyUI += UpdateVisualBar;
                    break;
                case AttributeType.Hunger:
                    GameEvents_PetCare.OnModifyHungerUI += UpdateVisualBar;
                    break;
            }

            GameEvents_PetCare.OnFinishedSimulation += StartUI;
        }

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        void OnDestroy()
        {
            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    GameEvents_PetCare.OnModifyGlycemiaUI -= UpdateVisualBar;
                    break;
                case AttributeType.Energy:
                    GameEvents_PetCare.OnModifyEnergyUI -= UpdateVisualBar;
                    break;
                case AttributeType.Hunger:
                    GameEvents_PetCare.OnModifyHungerUI -= UpdateVisualBar;
                    break;
            }

            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                switch (_attributeType)
                {
                    case AttributeType.Glycemia:
                        GameEvents_PetCare.OnModifyGlycemiaUI -= UpdateVisualBar;
                        break;
                    case AttributeType.Energy:
                        GameEvents_PetCare.OnModifyEnergyUI -= UpdateVisualBar;
                        break;
                    case AttributeType.Hunger:
                        GameEvents_PetCare.OnModifyHungerUI -= UpdateVisualBar;
                        break;
                }

                GameEvents_PetCare.OnFinishedSimulation -= StartUI;
            }
        }

        void OnApplicationQuit()
        {
            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    GameEvents_PetCare.OnModifyGlycemiaUI -= UpdateVisualBar;
                    break;
                case AttributeType.Energy:
                    GameEvents_PetCare.OnModifyEnergyUI -= UpdateVisualBar;
                    break;
                case AttributeType.Hunger:
                    GameEvents_PetCare.OnModifyHungerUI -= UpdateVisualBar;
                    break;
            }

            GameEvents_PetCare.OnFinishedSimulation -= StartUI;
        }
#endif

        void Start()
        {
            _petCareManager = ServiceLocator.Instance.GetService<IPetCareManager>();
        }

        private void ChangeValue(float value)
        {
            _slider.SetValueWithoutNotify(_currentValue);
        }

        private void InitializeVisualBar()
        {
            _slider.SetValueWithoutNotify(_currentValue);
            _value_TXT.text = $"{_slider.value} {_unit}";

            foreach (AttributeState state in _attributeStates)
            {
                if (_slider.value >= state.minValue && _slider.value <= state.maxValue)
                {
                    if (state.rangeValue == AttributeRangeValue.Good)
                    {
                        _sliderBackground.color = _colorGood;
                    }
                    else if (state.rangeValue == AttributeRangeValue.IntermediateLow || state.rangeValue == AttributeRangeValue.IntermediateHigh)
                    {
                        _sliderBackground.color = _colorIntermediate;
                    }
                    else if (state.rangeValue == AttributeRangeValue.BadLow || state.rangeValue == AttributeRangeValue.BadHigh)
                    {
                        _sliderBackground.color = _colorBad;
                    }
                    break;
                }
            }
        }

        private void UpdateVisualBar(int newValue)
        {
            if (!_isSimulationFinished)
                return;

            _currentValue = Mathf.Clamp(newValue, _minValue, _maxValue);
            _slider.SetValueWithoutNotify(_currentValue);
            _value_TXT.text = $"{_slider.value} {_unit}";

            foreach (AttributeState state in _attributeStates)
            {
                if (_slider.value >= state.minValue && _slider.value <= state.maxValue)
                {
                    if (state.rangeValue == AttributeRangeValue.Good)
                    {
                        _sliderBackground.color = _colorGood;
                    }
                    else if (state.rangeValue == AttributeRangeValue.IntermediateLow || state.rangeValue == AttributeRangeValue.IntermediateHigh)
                    {
                        _sliderBackground.color = _colorIntermediate;
                    }
                    else if (state.rangeValue == AttributeRangeValue.BadLow || state.rangeValue == AttributeRangeValue.BadHigh)
                    {
                        _sliderBackground.color = _colorBad;
                    }
                    break;
                }
            }
        }

        private void StartUI()
        {
            _isSimulationFinished = true;

            _slider = GetComponentInChildren<Slider>();
            _value_TXT = GetComponentInChildren<TextMeshProUGUI>();
            _slider.wholeNumbers = true;

            switch (_attributeType)
            {
                case AttributeType.Glycemia:
                    _currentValue = _petCareManager.glycemiaValue;
                    _attributeStates = _petCareManager.glycemiaRangeStates;
                    _minValue = _petCareManager.minGlycemiaValue;
                    _maxValue = _petCareManager.maxGlycemiaValue;
                    _unit = "mg/dL";
                    break;
                case AttributeType.Energy:
                    _currentValue = _petCareManager.energyValue;
                    _attributeStates = _petCareManager.energyRangeStates;
                    _minValue = _petCareManager.minEnergyValue;
                    _maxValue = _petCareManager.maxEnergyValue;
                    _unit = "%";
                    break;
                case AttributeType.Hunger:
                    _currentValue = _petCareManager.hungerValue;
                    _attributeStates = _petCareManager.hungerRangeStates;
                    _minValue = _petCareManager.minHungerValue;
                    _maxValue = _petCareManager.maxHungerValue;
                    _unit = "%";
                    break;
            }

            Transform background = transform.Find("Bar/Background");
            _sliderBackground = background.GetComponent<Image>();
            _slider.minValue = _minValue;
            _slider.maxValue = _maxValue;
            _value_TXT.text = $"{_slider.value} {_unit}";

            _slider.onValueChanged.AddListener(ChangeValue);
            _slider.value = _currentValue;
            InitializeVisualBar();
        }
    }
}