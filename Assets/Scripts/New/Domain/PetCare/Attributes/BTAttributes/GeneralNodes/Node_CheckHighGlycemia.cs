using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckHighGlycemia : Node
    {
        public Node_CheckHighGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsGlycemiaInRange(AttributeManager.Instance.glycemiaValue, "bad2"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}