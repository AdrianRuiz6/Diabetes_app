using UnityEngine;
using Master.Auxiliar;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_CheckUserPerformance : StateQuestions
    {
        private string _newState;
        private IQuestionManager _questionManager;
        private IUserPerformanceManager _userPerformanceManager;

        public State_CheckUserPerformance(IQuestionManager questionManager, IUserPerformanceManager userPerformanceManager)
        {
            _questionManager = questionManager;
            _userPerformanceManager = userPerformanceManager;
        }
        public override void Execute()
        {
            FixedSizeQueue<string> topicPerformance = new FixedSizeQueue<string>();
            bool isPerformanceComplete = true;

            foreach (string topic in _questionManager.allQuestions.Keys)
            {
                topicPerformance = _userPerformanceManager.GetTopicPerformance(topic);
                if (topicPerformance.Contains("P"))
                {
                    isPerformanceComplete = false;
                    break;
                }
            }

            if (isPerformanceComplete)
            {
                _newState = "ProcessQuestions";
                GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_ProcessQuestions(_questionManager, _userPerformanceManager));
            }
            else
            {
                _newState = "GenerateInitialQuestions";
                GameEvents_Questions.OnExecuteNextQuestionState?.Invoke(new State_GenerateInitialQuestions(_questionManager, _userPerformanceManager));
            }
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}