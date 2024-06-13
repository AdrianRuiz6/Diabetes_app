using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForRestart : State
{
    private string _newState;

    public override void Execute(AgentQuestions agent)
    {
        if (agent.executing == true)
        {
            _newState = "CheckUserPerformance";
            agent.Change_state(new CheckUserPerformance());
        }
    }

    public override void OnExit()
    {
        Debug.Log("State changed to: " + _newState);
    }
}
    
