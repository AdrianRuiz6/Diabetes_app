using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckFoodActive : Node
    {
        public Node_CheckFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            if (AttributeManager.Instance.isFoodEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
