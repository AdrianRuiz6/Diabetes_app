using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Master.Domain.Questions;
using Master.Domain.GameEvents;

namespace Master.Domain.Questions
{
    public class State_PresentQuestions : StateQuestions
    {
        private string _newState;

        public override void Execute(AgentQuestions agent)
        {
            QuestionManager.Instance.RandomizeOrderQuestions();

            GameEvents_Questions.OnFinalizedCreationQuestions?.Invoke();

            _newState = "WaitForRestart";
            agent.Change_state(new State_WaitForRestart());
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}