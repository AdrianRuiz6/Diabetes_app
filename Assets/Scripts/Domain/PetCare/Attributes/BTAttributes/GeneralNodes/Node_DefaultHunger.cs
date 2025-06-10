using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultHunger : Node
    {
        private IPetCareManager _petCareManager;

        public Node_DefaultHunger(IPetCareManager petCareManager) {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyHunger(2, currentTime);
            return NodeState.SUCCESS;
        }
    }
}
