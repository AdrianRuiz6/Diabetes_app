using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyBadHighEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyBadHighEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyGlycemia(7);
            return NodeState.SUCCESS;
        }
    }
}
