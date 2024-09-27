using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_CheckFoodActive : Node
    {
        public NodeHunger_CheckFoodActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.isFoodEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}