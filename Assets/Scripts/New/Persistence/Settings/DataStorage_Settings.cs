using System;
using UnityEngine;

namespace Master.Persistence.Settings
{
    public static class DataStorage_Settings
    {
        public static void SaveSoundEffectsVolume(float volume)
        {
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
            PlayerPrefs.Save();
        }

        public static float LoadSoundEffectsVolume()
        {
            return PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
        }

        public static void SaveInitialTime(TimeSpan initialTime)
        {
            int hour = initialTime.Hours;
            PlayerPrefs.SetInt("InitialTime", hour);
            PlayerPrefs.Save();
        }

        public static TimeSpan LoadInitialTime()
        {
            int hour = PlayerPrefs.GetInt("InitialTime", 14);
            return new TimeSpan(hour, 0, 0);
        }

        public static void SaveFinishTime(TimeSpan finishTime)
        {
            int hour = finishTime.Hours + 1;
            PlayerPrefs.SetInt("FinishTime", hour);
            PlayerPrefs.Save();
        }

        public static TimeSpan LoadFinishTime()
        {
            int hour = PlayerPrefs.GetInt("FinishTime", 21);
            return new TimeSpan(hour - 1, 59, 0);
        }
    }
}