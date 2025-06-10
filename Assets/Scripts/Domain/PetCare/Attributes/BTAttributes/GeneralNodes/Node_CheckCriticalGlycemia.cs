using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckCriticalGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckCriticalGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (_petCareManager.IsGlycemiaInRange(AttributeRangeValue.BadLow))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}