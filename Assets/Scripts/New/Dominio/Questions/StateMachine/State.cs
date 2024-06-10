using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Execute(CAgent agent);
    public abstract void OnExit();
}
