using System;
using UnityEngine;
using Master.Persistence;
using Master.Domain.GameEvents;
using Master.Persistence.Settings;
using Master.Domain.PetCare;
using Master.Domain.Score;
using Master.Persistence.PetCare;
using Master.Presentation.PetCare.Log;

namespace Master.Domain.Settings
{
    public class SettingsManager
    {
        public TimeSpan initialTime { get; private set; }
        public TimeSpan finishTime { get; private set; }

        public float soundEffectsVolume { get; private set; }

        public SettingsManager()
        {
            initialTime = DataStorage_Settings.LoadInitialTime();

            finishTime = DataStorage_Settings.LoadFinishTime();

            soundEffectsVolume = DataStorage_Settings.LoadSoundEffectsVolume();
        }

        public void SetInitialHour(int newHour)
        {
            initialTime = new TimeSpan(newHour, 0, 0);
            GameEvents_Settings.OnInitialTimeModified?.Invoke(newHour);
            DataStorage_Settings.SaveInitialTime(initialTime);
        }

        public void SetFinishHour(int newHour)
        {
            finishTime = new TimeSpan(newHour, 59, 0);
            GameEvents_Settings.OnFinishTimeModified?.Invoke(newHour);
            DataStorage_Settings.SaveFinishTime(finishTime);
        }

        public bool IsInRange(TimeSpan currentTime)
        {
            if (finishTime >= initialTime)
            {
                return currentTime >= initialTime && currentTime <= finishTime;
            }
            else
            {
                return currentTime >= initialTime || currentTime <= finishTime;
            }
        }

        private void SetSoundEffectsVolume(float volume)
        {
            soundEffectsVolume = volume;
            GameEvents_Settings.OnSoundEffectsModified?.Invoke(soundEffectsVolume);
            DataStorage_Settings.SaveSoundEffectsVolume(soundEffectsVolume);
        }

        private void ConfirmChangeRangeTime(float currentInitialHour, float currentFinishHour)
        {
            SetInitialHour((int)currentInitialHour);
            SetFinishHour((int)currentFinishHour);

            // Reset score
            ScoreManager.Instance.ResetScore();

            // Reset attributes
            AttributeManager.Instance.RestartGlycemia(DateTime.Now);
            AttributeManager.Instance.RestartActivity(DateTime.Now);
            AttributeManager.Instance.RestartHunger(DateTime.Now);

            // Reset attributes record
            DataStorage_PetCare.ResetActivityLog();
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Activity);
            DataStorage_PetCare.ResetHungerLog();
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Hunger);
            DataStorage_PetCare.ResetGlycemiaLog();
            GameEvents_PetCareLog.OnChangedAttributeTypeFilter?.Invoke(AttributeType.Glycemia);

            // Reset actions record
            DataStorage_PetCare.ResetInsulinLog();
            DataStorage_PetCare.ResetFoodLog();
            DataStorage_PetCare.ResetExerciseLog();
            GameEvents_PetCareLog.OnUpdatedActionsLog?.Invoke();

            // Reset actions CD and effects
            AttributeManager.Instance.DeactivateInsulinActionCD();
            AttributeManager.Instance.DeactivateInsulinEffect();
            AttributeManager.Instance.DeactivateExerciseActionCD();
            AttributeManager.Instance.DeactivateExerciseEffect();
            AttributeManager.Instance.DeactivateFoodActionCD();
            AttributeManager.Instance.DeactivateFoodEffect();
            GameEvents_PetCare.OnFinishTimerCD?.Invoke();
        }
    }
}