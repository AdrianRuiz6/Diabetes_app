using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckBadLowEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckBadLowEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsEnergyInRange(AttributeRangeValue.BadLow, intervalInfo.energyValue))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
