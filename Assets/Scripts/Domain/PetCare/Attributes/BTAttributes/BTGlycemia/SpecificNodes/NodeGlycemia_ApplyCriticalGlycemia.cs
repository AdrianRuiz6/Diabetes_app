using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyCriticalGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyCriticalGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyGlycemia(20, currentTime);
            return NodeState.SUCCESS;
        }
    }
}