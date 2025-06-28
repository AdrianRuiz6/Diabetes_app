using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckBadHighEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckBadHighEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsEnergyInRange(AttributeRangeValue.BadHigh, intervalInfo.energyValue))
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}