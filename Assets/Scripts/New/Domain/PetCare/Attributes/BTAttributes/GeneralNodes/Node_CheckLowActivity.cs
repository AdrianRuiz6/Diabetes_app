using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckLowActivity : Node
    {
        public Node_CheckLowActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsActivityInRange(AttributeManager.Instance.activityValue, "bad1"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
