using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckIntermediateLowGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckIntermediateLowGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsGlycemiaInRange(AttributeRangeValue.IntermediateLow, intervalInfo.glycemiaValue))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}