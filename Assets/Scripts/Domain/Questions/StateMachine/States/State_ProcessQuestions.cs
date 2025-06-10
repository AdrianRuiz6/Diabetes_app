using System.Collections.Generic;
using UnityEngine;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_ProcessQuestions : StateQuestions
    {
        private string _newState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        public State_ProcessQuestions(IQuestionManager questionManager, IUserPerformanceManager userPerformanceManager)
        {
            _questionManager = questionManager;
            _userPerformanceManager = userPerformanceManager;
        }
        public override void Execute()
        {
            Dictionary<string, float> appareanceProportions = new Dictionary<string, float>();
            Dictionary<string, int> questionCount = new Dictionary<string, int>();

            appareanceProportions = _questionManager.CalculateAppearanceProportions();
            questionCount = _questionManager.AdjustQuestionCount(appareanceProportions);

            _questionManager.SelectRandomQuestions(amountQuestionsPerTopic: questionCount);

            _newState = "PresentQuestions";
            GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_PresentQuestions(_questionManager, _userPerformanceManager));
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}