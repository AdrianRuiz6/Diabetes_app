using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.Questions;

namespace Master.Domain.Questions
{
    public class State_ProcessQuestions : StateQuestions
    {
        private string _newState;

        public override void Execute(AgentQuestions agent)
        {
            Dictionary<string, float> appareanceProportions = new Dictionary<string, float>();
            Dictionary<string, int> questionCount = new Dictionary<string, int>();

            appareanceProportions = QuestionManager.Instance.CalculateAppearanceProportions();
            questionCount = QuestionManager.Instance.AdjustQuestionCount(appareanceProportions);

            QuestionManager.Instance.SelectRandomQuestions(amountQuestionsPerTopic: questionCount);

            _newState = "PresentQuestions";
            agent.Change_state(new State_PresentQuestions());
        }

        public override void OnExit()
        {
            Debug.Log("State changed to: " + _newState);
        }
    }
}