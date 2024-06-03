using System.Collections;
using System.Collections.Generic;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckLowActivity : Node
    {
        public Node_CheckLowActivity() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.activityValue <= 20)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
