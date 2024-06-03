using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_ApplyCriticalGlycemia : Node
    {
        public Node_ApplyCriticalGlycemia() { }

        public override NodeState Evaluate()
        {
            AttributeManager.Instance.ModifyGlycemia(20);
            return NodeState.SUCCESS;
        }
    }
}