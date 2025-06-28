using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeEnergy_ApplyBadLowGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public NodeEnergy_ApplyBadLowGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyEnergy(-5);
            return NodeState.SUCCESS;
        }
    }
}
