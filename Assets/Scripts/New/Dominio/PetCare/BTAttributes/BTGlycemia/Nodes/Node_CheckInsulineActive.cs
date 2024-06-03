using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckInsulineActive : Node
    {
        public Node_CheckInsulineActive() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.isInsulineButtonUsed)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
