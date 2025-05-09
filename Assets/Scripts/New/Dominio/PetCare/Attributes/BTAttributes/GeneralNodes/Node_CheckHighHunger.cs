using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckHighHunger : Node
    {
        public Node_CheckHighHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsHungerInRange(AttributeManager.Instance.hungerValue, "bad2"))
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}