using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckIntermediateHighEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckIntermediateHighEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            if (_petCareManager.IsEnergyInRange(AttributeRangeValue.IntermediateHigh, intervalInfo.energyValue))
            {
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }
}