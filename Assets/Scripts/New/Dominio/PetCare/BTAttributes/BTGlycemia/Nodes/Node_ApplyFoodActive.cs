using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_ApplyFoodActive : Node
    {
        public Node_ApplyFoodActive() { }

        public override NodeState Evaluate()
        {
            AttributeManager.Instance.ModifyGlycemia(+5);
            return NodeState.SUCCESS;
        }
    }
}