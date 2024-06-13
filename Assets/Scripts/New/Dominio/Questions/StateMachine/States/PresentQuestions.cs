using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class PresentQuestions : State // TODO: aqui se barajean las respuestas y las preguntas.
{
    private string _newState;

    public override void Execute(AgentQuestions agent)
    {
        QuestionManager.Instance.RandomizeOrderQuestions();

        GameEventsQuestions.OnFinalizedCreationQuestions?.Invoke();

        _newState = "WaitForRestart";
        agent.Change_state(new WaitForRestart());
    }

    public override void OnExit()
    {
        Debug.Log("State changed to: " + _newState);
    }
}
