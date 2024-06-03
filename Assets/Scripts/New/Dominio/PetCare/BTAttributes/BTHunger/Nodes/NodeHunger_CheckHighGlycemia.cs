using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_CheckHighGlycemia : Node
    {
        public NodeHunger_CheckHighGlycemia() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.glycemiaValue >= 250)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}