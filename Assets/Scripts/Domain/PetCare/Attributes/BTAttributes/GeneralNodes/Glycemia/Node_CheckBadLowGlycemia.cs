using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckBadLowGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckBadLowGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsGlycemiaInRange(AttributeRangeValue.BadLow, intervalInfo.glycemiaValue))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}