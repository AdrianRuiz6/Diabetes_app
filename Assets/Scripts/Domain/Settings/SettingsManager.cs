using System;
using Master.Domain.GameEvents;
using Master.Domain.PetCare;
using Master.Domain.PetCare.Log;
using Master.Domain.Questions;
using Master.Domain.Score;

namespace Master.Domain.Settings
{
    public class SettingsManager : ISettingsManager
    {
        private ISettingsRepository _settingsRepository;
        private IPetCareManager _petCareManager;
        private IPetCareLogManager _petCareLogManager;
        private IQuestionRepository _questionRepository;
        private IQuestionManager _questionManager;
        private IScoreManager _scoreManager;
        private IScoreLogManager _scoreLogManager;

        public TimeSpan initialTime { get; private set; }
        public TimeSpan finishTime { get; private set; }

        public float soundEffectsVolume { get; private set; }

        public SettingsManager(ISettingsRepository settingsRepository, IQuestionRepository questionRepository)
        {
            _settingsRepository = settingsRepository;
            _questionRepository = questionRepository;
            initialTime = _settingsRepository.LoadInitialTime();

            finishTime = _settingsRepository.LoadFinishTime();

            soundEffectsVolume = _settingsRepository.LoadSoundEffectsVolume();
        }

        public void InitializeDependencies(IPetCareManager petCareManager, IPetCareLogManager petCareLogManager, IQuestionManager questionManager, IScoreManager scoreManager, IScoreLogManager scoreLogManager)
        {
            _petCareManager = petCareManager;
            _petCareLogManager = petCareLogManager;
            _questionManager = questionManager;
            _scoreManager = scoreManager;
            _scoreLogManager = scoreLogManager;
        }

        public void SetInitialHour(int newHour)
        {
            initialTime = new TimeSpan(newHour, 0, 0);
            GameEvents_Settings.OnInitialTimeModified?.Invoke(newHour);
            _settingsRepository.SaveInitialTime(initialTime);
        }

        public void SetFinishHour(int newHour)
        {
            finishTime = new TimeSpan(newHour, 59, 0);
            GameEvents_Settings.OnFinishTimeModified?.Invoke(newHour);
            _settingsRepository.SaveFinishTime(finishTime);
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

        public void SetSoundEffectsVolume(float volume)
        {
            soundEffectsVolume = volume;
            GameEvents_Settings.OnSoundEffectsModified?.Invoke(soundEffectsVolume);
            _settingsRepository.SaveSoundEffectsVolume(soundEffectsVolume);
        }

        public void ConfirmChangeRangeTime(float currentInitialHour, float currentFinishHour)
        {
            SetInitialHour((int)currentInitialHour);
            SetFinishHour((int)currentFinishHour);

            // Reset score
            _scoreManager.ResetScore();
            _scoreLogManager.ClearScoreLogElements();

            // Reset attributes record
            _petCareLogManager.ClearThisDateAttributeLog(AttributeType.Energy);
            _petCareLogManager.ClearThisDateAttributeLog(AttributeType.Hunger);
            _petCareLogManager.ClearThisDateAttributeLog(AttributeType.Glycemia);

            // Reset actions record
            _petCareLogManager.ClearThisDateActionLog(ActionType.Insulin);
            _petCareLogManager.ClearThisDateActionLog(ActionType.Food);
            _petCareLogManager.ClearThisDateActionLog(ActionType.Exercise);

            // Reset attributes
            _petCareManager.RestartGlycemia(DateTime.Now);
            _petCareManager.RestartEnergy(DateTime.Now);
            _petCareManager.RestartHunger(DateTime.Now);

            // Reset actions CD and effects
            _petCareManager.DeactivateInsulinActionCD();
            _petCareManager.DeactivateInsulinEffect();
            _petCareManager.DeactivateExerciseActionCD();
            _petCareManager.DeactivateExerciseEffect();
            _petCareManager.DeactivateFoodActionCD();
            _petCareManager.DeactivateFoodEffect();
        }

        public int TryChangingQuestionsURL(string input)
        {
            return _questionRepository.SaveURLQuestions(input);
        }

        public bool ChangeQuestions()
        {
            // Se reinician valores antes de buscar nuevas preguntas
            ResetQuestionsValues();

            // Se vuelven a buscar preguntas
            return _questionManager.InitializeQuestions(true);
        }

        public bool ResetQuestions()
        {
            // Se reinician valores antes de buscar nuevas preguntas
            _questionRepository.ResetQuestionURL();
            ResetQuestionsValues();

            // Se vuelven a buscar preguntas
            return _questionManager.InitializeQuestions(true);
        }

        private void ResetQuestionsValues()
        {
            _questionRepository.ResetUserPerformance();
            _questionRepository.ResetIterationQuestions();
            _questionRepository.SaveCurrentQuestionIndex(0);
        }
    }
}