using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeActivity_ApplyHighHunger : Node
    {
        private IPetCareManager _petCareManager;

        public NodeActivity_ApplyHighHunger(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyActivity(-5, currentTime);
            return NodeState.SUCCESS;
        }
    }
}