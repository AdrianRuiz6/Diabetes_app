using System.Collections;
using System.Collections.Generic;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckLowActivity : Node
    {
        public NodeGlycemia_CheckLowActivity() { }

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
