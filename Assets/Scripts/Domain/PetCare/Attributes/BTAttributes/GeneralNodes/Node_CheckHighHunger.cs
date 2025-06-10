using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckHighHunger : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckHighHunger(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (_petCareManager.IsHungerInRange(AttributeRangeValue.BadHigh))
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}