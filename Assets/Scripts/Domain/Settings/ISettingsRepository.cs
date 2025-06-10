using System;

namespace Master.Domain.Settings
{
    public interface ISettingsRepository
    {
        public void SaveSoundEffectsVolume(float volume);

        public float LoadSoundEffectsVolume();

        public void SaveInitialTime(TimeSpan initialTime);

        public TimeSpan LoadInitialTime();

        public void SaveFinishTime(TimeSpan finishTime);

        public TimeSpan LoadFinishTime();
    }
}