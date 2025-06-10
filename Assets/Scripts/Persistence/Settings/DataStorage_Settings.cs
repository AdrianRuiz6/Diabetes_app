using Master.Domain.Settings;
using System;
using UnityEngine;

namespace Master.Persistence.Settings
{
    public class DataStorage_Settings : ISettingsRepository
    {
        public void SaveSoundEffectsVolume(float volume)
        {
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
            PlayerPrefs.Save();
        }

        public float LoadSoundEffectsVolume()
        {
            return PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
        }

        public void SaveInitialTime(TimeSpan initialTime)
        {
            int hour = initialTime.Hours;
            PlayerPrefs.SetInt("InitialTime", hour);
            PlayerPrefs.Save();
        }

        public TimeSpan LoadInitialTime()
        {
            int hour = PlayerPrefs.GetInt("InitialTime", 14);
            return new TimeSpan(hour, 0, 0);
        }

        public void SaveFinishTime(TimeSpan finishTime)
        {
            int hour = finishTime.Hours + 1;
            PlayerPrefs.SetInt("FinishTime", hour);
            PlayerPrefs.Save();
        }

        public TimeSpan LoadFinishTime()
        {
            int hour = PlayerPrefs.GetInt("FinishTime", 21);
            return new TimeSpan(hour - 1, 59, 0);
        }
    }
}