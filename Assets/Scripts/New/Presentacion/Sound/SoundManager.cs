using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound effects")]
    [SerializeField] private AudioSource _soundEffectsAudioSource;
    [SerializeField] private List<SoundEffect> _soundEffects;
    private Dictionary<string, AudioClip> _soundEffectsDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        DataStorage.SaveSoundEffectsVolume(_soundEffectsAudioSource.volume);
    }

    private void Start()
    {
        SetSoundEffectsVolume(DataStorage.LoadSoundEffectsVolume());

        // Inicialización del diccionario de efectos.
        _soundEffectsDictionary = new Dictionary<string, AudioClip>();
        foreach (SoundEffect sound in _soundEffects)
        {
            _soundEffectsDictionary[sound.name] = sound.clip;
        }

    }

    public void PlaySoundEffect(string soundName)
    {
        if (_soundEffectsDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            _soundEffectsAudioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Efecto de sonido '{soundName}' no encontrado.");
        }
    }

    public void SetSoundEffectsVolume(float volume)
    {
        _soundEffectsAudioSource.volume = volume;
    }
}
