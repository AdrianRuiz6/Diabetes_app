using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_ApplyDefaultHunger : Node
    {
        private IPetCareManager _petCareManager;

        public Node_ApplyDefaultHunger(IPetCareManager petCareManager) {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyHunger(2);
            return NodeState.SUCCESS;
        }
    }
}
