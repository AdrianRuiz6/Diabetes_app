using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckFoodActive : Node
    {
        public Node_CheckFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            if (AttributeManager.Instance.isFoodEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
