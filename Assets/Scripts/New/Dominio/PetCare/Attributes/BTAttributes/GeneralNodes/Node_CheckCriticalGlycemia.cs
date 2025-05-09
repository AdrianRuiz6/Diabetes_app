using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckCriticalGlycemia : Node
    {
        public Node_CheckCriticalGlycemia(){}

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsGlycemiaInRange(AttributeManager.Instance.glycemiaValue, "critical"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}