using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckIntermediateLowEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckIntermediateLowEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsEnergyInRange(AttributeRangeValue.IntermediateLow, intervalInfo.energyValue))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
