using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Questions;

namespace Master.Domain.Questions
{
    public class State_GenerateInitialQuestions : StateQuestions
    {
        private string _newState;

        public override void Execute(AgentQuestions agent)
        {
            QuestionManager.Instance.SelectRandomQuestions();

            _newState = "PresentQuestions";
            agent.Change_state(new State_PresentQuestions());
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}