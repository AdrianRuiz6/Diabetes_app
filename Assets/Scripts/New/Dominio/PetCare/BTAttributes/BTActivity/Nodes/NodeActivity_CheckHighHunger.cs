using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeActivity_CheckHighHunger : Node
    {
        public NodeActivity_CheckHighHunger() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.hungerValue >= 80)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}