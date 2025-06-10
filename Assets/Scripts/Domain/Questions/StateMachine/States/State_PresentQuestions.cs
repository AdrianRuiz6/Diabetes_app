using UnityEngine;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_PresentQuestions : StateQuestions
    {
        private string _newState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        public State_PresentQuestions(IQuestionManager questionManager, IUserPerformanceManager userPerformanceManager)
        {
            _questionManager = questionManager;
            _userPerformanceManager = userPerformanceManager;
        }
        public override void Execute()
        {
            _questionManager.RandomizeOrderQuestions();

            _questionManager.FinishQuestionSearch();

            _newState = "WaitForRestart";
            GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_WaitForRestart(_questionManager, _userPerformanceManager));
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}