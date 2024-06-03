using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeSequence : Node   //Nodo secuencia ejecución paralela;
    {
        public NodeSequence() : base() { }
        public NodeSequence(List<Node> children):base(children){}

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            //Debug.Log("Evaluating en Node Secuencia");
           foreach (Node child in children)
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
