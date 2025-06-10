using UnityEngine;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_GenerateInitialQuestions : StateQuestions
    {
        private string _newState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        public State_GenerateInitialQuestions(IQuestionManager questionManager, IUserPerformanceManager userPerformanceManager)
        {
            _questionManager = questionManager;
            _userPerformanceManager = userPerformanceManager;
        }
        public override void Execute()
        {
            _questionManager.SelectRandomQuestions();

            _newState = "PresentQuestions";
            GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_PresentQuestions(_questionManager, _userPerformanceManager));
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}