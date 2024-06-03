using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckFoodActive : Node
    {
        public Node_CheckFoodActive() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.isFoodButtonUsed)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
