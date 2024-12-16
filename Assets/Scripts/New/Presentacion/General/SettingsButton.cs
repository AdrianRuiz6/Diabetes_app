using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsButton : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Button _closeSettingsButton;

    [Header("Volume")]
    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSoundEffects;

    [Header("Limit hours")] // TODO
    [SerializeField] private Slider _sliderInitialHour;
    [SerializeField] private TMP_Text _initialHour_TMP;
    [SerializeField] private Slider _sliderFinishHour;
    [SerializeField] private TMP_Text _finishHour_TMP;

    void Start()
    {
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

        // Configuración de los botones.
        _openSettingsButton.onClick.AddListener(OpenSetting);
        _closeSettingsButton.onClick.AddListener(CloseSetting);
    }

    private void OpenSetting()
    {
        _settingsPanel.SetActive(true);
    }

    private void CloseSetting()
    {
        _settingsPanel?.SetActive(false);
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
        GameEventsGraph.OnInitialTimeModified?.Invoke((int)hour);
        SetInitialHourTMP(hour);
    }

    private void SetInitialHourTMP(float hour)
    {
        string hourText = (hour < 10) ? $"0{hour}" : hour.ToString();
        _initialHour_TMP.text = $"{hourText}:00";
    }

    private void ChangeFinishHour(float hour)
    {
        GameEventsGraph.OnFinishTimeModified?.Invoke((int)hour);
        SetFinishHourTMP(hour);
    }

    private void SetFinishHourTMP(float hour)
    {
        string hourText = (hour < 10) ? $"0{hour}" : hour.ToString();
        _finishHour_TMP.text = $"{hourText}:59";
    }
}
