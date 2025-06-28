using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyBadLowGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyBadLowGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyHunger(5);
            return NodeState.SUCCESS;
        }
    }
}