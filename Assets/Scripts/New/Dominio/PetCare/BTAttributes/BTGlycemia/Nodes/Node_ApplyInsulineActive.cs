using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_ApplyInsulineActive : Node
    {
        public Node_ApplyInsulineActive() { }

        public override NodeState Evaluate()
        {
            AttributeManager.Instance.ModifyGlycemia(-5);
            return NodeState.SUCCESS;
        }
    }
}