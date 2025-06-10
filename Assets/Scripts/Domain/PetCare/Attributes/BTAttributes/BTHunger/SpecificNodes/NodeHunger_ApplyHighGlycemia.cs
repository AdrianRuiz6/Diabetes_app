using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyHighGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyHighGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyGlycemia(-5, currentTime);
            return NodeState.SUCCESS;
        }
    }
}