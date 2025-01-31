using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_CheckLowGlycemia : Node
    {
        public NodeHunger_CheckLowGlycemia() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            if (AttributeManager.Instance.IsGlycemiaInRange(AttributeManager.Instance.glycemiaValue, "bad1"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}