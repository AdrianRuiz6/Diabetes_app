using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sound effects")]
    [SerializeField] private AudioSource _soundEffectsAudioSource;
    [SerializeField] private List<SoundEffect> _soundEffects;
    private Dictionary<string, AudioClip> _soundEffectsDictionary;
    
    [Header("Music")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioClip _musicClip;

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

        // Inicialización del diccionario de efectos.
        _soundEffectsDictionary = new Dictionary<string, AudioClip>();
        foreach (SoundEffect sound in _soundEffects)
        {
            _soundEffectsDictionary[sound.name] = sound.clip;
        }

        // Configuración del AudioSource de música.
        _musicAudioSource.loop = true;
        _musicAudioSource.clip = _musicClip;
        _musicAudioSource.Play();
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

    public void SetMusicVolume(float volume)
    {
        _musicAudioSource.volume = volume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        _soundEffectsAudioSource.volume = volume;
    }
}
