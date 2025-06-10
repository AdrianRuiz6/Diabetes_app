using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyLowActivity : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyLowActivity(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyGlycemia(10, currentTime);
            return NodeState.SUCCESS;
        }
    }
}