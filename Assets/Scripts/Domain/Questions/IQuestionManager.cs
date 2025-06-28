using System.Collections.Generic;

namespace Master.Domain.Questions
{
    public interface IQuestionManager
    {
        public Dictionary<string, List<Question>> allQuestions {get; }
        public int currentQuestionIndex { get; }
        public float maxTimerSeconds { get; }

        public bool isFSMExecuting { get; }
        public bool InitializeQuestions(bool isChangingQuestions = false);

        public void StartQuestionSearch();

        public void FinishQuestionSearch();

        public void FinishTimerQuestions();

        public void InitializeTimerQuestions();

        public Dictionary<string, float> CalculateAppearanceProportions();

        public Dictionary<string, int> AdjustQuestionCount(Dictionary<string, float> appearanceProportions);

        public void SelectRandomQuestions(Dictionary<string, int> amountQuestionsPerTopic = null);

        public void RandomizeOrderQuestions();

        public Question GetNextQuestion();

        public void SaveTimeLeftQuestionTimer(float time);

        public void Answer(string answerText);
    }
}