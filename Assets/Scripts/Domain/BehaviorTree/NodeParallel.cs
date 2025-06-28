using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree

{
    public class NodeParallel : Node
    {
        public NodeParallel() : base() { }
        public NodeParallel(List<Node> children) : base(children) { }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            bool hasSuccess = false;
            bool anyChildIsRunning = false;
            foreach (Node child in children)
            {
                switch (child.Evaluate(intervalInfo))
                {
                    case NodeState.FAILURE:
                        break;
                    case NodeState.SUCCESS:
                        hasSuccess = true;
                        break;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        break;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            if (anyChildIsRunning)
            {
                state = NodeState.RUNNING;
            }
            else if (hasSuccess)
            {
                state = NodeState.SUCCESS;
            }
            else
            {
                state = NodeState.FAILURE;
            }

            return state;
        }
    }
}