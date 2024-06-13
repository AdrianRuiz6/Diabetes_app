using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUserPerformance : State
{
    private string _newState;

    public override void Execute(AgentQuestions agent)
    {
        FixedSizeQueue<char> topicPerformance = new FixedSizeQueue<char>();
        bool isPerformanceComplete = true;

        foreach (string topic in QuestionManager.Instance._allQuestions.Keys)
        {
            topicPerformance = UserPerformanceManager.Instance.GetTopicPerformance(topic);
            if (topicPerformance.Contains('P'))
            {
                isPerformanceComplete = false;
                break;
            }
        }

        if (isPerformanceComplete)
        {
            _newState = "ProcessQuestions";
            agent.Change_state(new ProcessQuestions());
        }
        else
        {
            _newState = "GenerateInitialQuestions";
            agent.Change_state(new GenerateInitialQuestions());
        }
    }

    public override void OnExit()
    {
        Debug.Log("State changed to: " + _newState);
    }
}
