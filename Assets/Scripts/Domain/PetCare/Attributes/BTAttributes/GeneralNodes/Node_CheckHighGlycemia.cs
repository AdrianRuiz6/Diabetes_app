using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckHighGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckHighGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (_petCareManager.IsGlycemiaInRange(AttributeRangeValue.BadHigh))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}