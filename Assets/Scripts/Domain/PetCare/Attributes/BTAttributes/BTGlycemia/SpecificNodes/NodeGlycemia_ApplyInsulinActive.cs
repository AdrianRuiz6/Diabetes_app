using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyInsulinActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyInsulinActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyGlycemia(-2);
            return NodeState.SUCCESS;
        }
    }
}