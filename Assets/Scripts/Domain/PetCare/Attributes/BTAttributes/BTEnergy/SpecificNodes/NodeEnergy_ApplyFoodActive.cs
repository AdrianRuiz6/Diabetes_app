using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeEnergy_ApplyFoodActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeEnergy_ApplyFoodActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyEnergy(2);
            return NodeState.SUCCESS;
        }
    }
}