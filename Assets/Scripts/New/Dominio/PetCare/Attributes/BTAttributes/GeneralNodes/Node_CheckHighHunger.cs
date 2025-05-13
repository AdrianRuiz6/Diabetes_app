using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckHighHunger : Node
    {
        public Node_CheckHighHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsHungerInRange(AttributeManager.Instance.hungerValue, "bad2"))
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}