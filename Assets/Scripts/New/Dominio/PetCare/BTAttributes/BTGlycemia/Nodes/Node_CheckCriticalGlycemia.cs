using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckCriticalGlycemia : Node
    {
        public Node_CheckCriticalGlycemia(){}

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.glycemiaValue <= 40)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}