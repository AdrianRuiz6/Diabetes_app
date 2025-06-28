using Master.Domain.GameEvents;
using Master.Domain.PetCare.Log;
using Master.Domain.PetCare;
using System;
using Master.Domain.Questions;
using Master.Domain.Score;

namespace Master.Domain.Settings
{
    public interface ISettingsManager
    {
        public TimeSpan initialTime { get; }
        public TimeSpan finishTime { get; }

        public float soundEffectsVolume { get; }

        public void InitializeDependencies(IPetCareManager petCareManager, IPetCareLogManager petCareLogManager, IQuestionManager questionManager, IScoreManager scoreManager, IScoreLogManager scoreLogManager);

        public void SetInitialHour(int newHour);

        public void SetFinishHour(int newHour);

        public bool IsInRange(TimeSpan currentTime);

        public void SetSoundEffectsVolume(float volume);

        public void ConfirmChangeRangeTime(float currentInitialHour, float currentFinishHour);

        public int TryChangingQuestionsURL(string input);

        public bool ChangeQuestions();

        public bool ResetQuestions();
    }
}