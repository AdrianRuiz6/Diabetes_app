using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class NodeSelector : Node
    {
        public NodeSelector() : base() { }
        public NodeSelector(List<Node> children): base(children) { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            foreach (Node child in children)
                switch (child.Evaluate(currentTime))
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            state = NodeState.FAILURE;
            return state;
        }
    }
}
