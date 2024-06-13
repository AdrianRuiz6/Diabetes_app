using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateInitialQuestions : State
{
    private string _newState;

    public override void Execute(AgentQuestions agent)
    {
        QuestionManager.Instance.SelectRandomQuestions();

        _newState = "PresentQuestions";
        agent.Change_state(new PresentQuestions());
    }

    public override void OnExit()
    {
        Debug.Log("State changed to: " + _newState);
    }
}
