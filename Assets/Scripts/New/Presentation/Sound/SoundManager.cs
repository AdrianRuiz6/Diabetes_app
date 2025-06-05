using System.Collections.Generic;
using UnityEngine;
using Master.Persistence;
using Master.Persistence.Settings;
using Master.Domain.GameEvents;

namespace Master.Presentation.Sound
{
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

            GameEvents_Settings.OnSoundEffectsModified += SetSoundEffectsVolume;
        }

        private void OnDestroy()
        {
            GameEvents_Settings.OnSoundEffectsModified -= SetSoundEffectsVolume;
        }

        private void Start()
        {
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

        private void SetSoundEffectsVolume(float volume)
        {
            _soundEffectsAudioSource.volume = volume;
        }
    }
}