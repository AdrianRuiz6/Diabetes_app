using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeParallel : Node
{
    public NodeParallel() : base() { }
    public NodeParallel(List<Node> children) : base(children) { }

    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;
        foreach (Node child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.FAILURE:
                    continue;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildIsRunning = true;
                    continue;
                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }
        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}

