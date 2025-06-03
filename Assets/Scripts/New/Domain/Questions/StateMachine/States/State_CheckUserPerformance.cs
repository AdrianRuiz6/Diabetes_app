using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Auxiliar;
using Master.Domain.Questions;

namespace Master.Domain.Questions
{
    public class State_CheckUserPerformance : StateQuestions
    {
        private string _newState;

        public override void Execute(AgentQuestions agent)
        {
            FixedSizeQueue<string> topicPerformance = new FixedSizeQueue<string>();
            bool isPerformanceComplete = true;

            foreach (string topic in QuestionManager.Instance._allQuestions.Keys)
            {
                topicPerformance = UserPerformanceManager.Instance.GetTopicPerformance(topic);
                if (topicPerformance.Contains("P"))
                {
                    isPerformanceComplete = false;
                    break;
                }
            }

            if (isPerformanceComplete)
            {
                _newState = "ProcessQuestions";
                agent.Change_state(new State_ProcessQuestions());
            }
            else
            {
                _newState = "GenerateInitialQuestions";
                agent.Change_state(new State_GenerateInitialQuestions());
            }
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}