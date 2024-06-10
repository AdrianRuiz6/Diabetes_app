using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentQuestions : State // TODO: aqui se barajean las respuestas y las preguntas.
{
    private string _newState;

    public override void Execute(CAgent agent)
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        Debug.Log("State changed to: " + _newState);
    }
}
