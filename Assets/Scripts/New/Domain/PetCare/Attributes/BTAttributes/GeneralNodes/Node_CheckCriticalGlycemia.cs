using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckCriticalGlycemia : Node
    {
        public Node_CheckCriticalGlycemia(){}

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsGlycemiaInRange(AttributeManager.Instance.glycemiaValue, "critical"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}