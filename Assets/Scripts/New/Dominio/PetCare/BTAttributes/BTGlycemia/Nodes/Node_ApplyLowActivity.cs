using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_ApplyLowActivity : Node
    {
        public Node_ApplyLowActivity() { }

        public override NodeState Evaluate()
        {
            AttributeManager.Instance.ModifyGlycemia(+10);
            return NodeState.SUCCESS;
        }
    }
}