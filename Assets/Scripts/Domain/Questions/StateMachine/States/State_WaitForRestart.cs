using UnityEngine;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_WaitForRestart : StateQuestions
    {
        private string _newState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        public State_WaitForRestart(IQuestionManager questionManager, IUserPerformanceManager userPerformanceManager)
        {
            _questionManager = questionManager;
            _userPerformanceManager = userPerformanceManager;
        }

        public override void Execute()
        {
            if (_questionManager.isFSMExecuting == true)
            {
                _newState = "CheckUserPerformance";
                GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_CheckUserPerformance(_questionManager, _userPerformanceManager));
            }
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}