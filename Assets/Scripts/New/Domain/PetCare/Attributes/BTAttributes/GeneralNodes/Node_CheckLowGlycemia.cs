using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckLowGlycemia : Node
    {
        public Node_CheckLowGlycemia() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            if (AttributeManager.Instance.IsGlycemiaInRange(AttributeManager.Instance.glycemiaValue, "bad1"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}