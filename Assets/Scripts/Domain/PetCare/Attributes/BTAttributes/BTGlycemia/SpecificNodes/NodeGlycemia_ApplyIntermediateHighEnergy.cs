using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyIntermediateHighEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyIntermediateHighEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyGlycemia(5);
            return NodeState.SUCCESS;
        }
    }
}