using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Execute(AgentQuestions agent);
    public abstract void OnExit();
}
