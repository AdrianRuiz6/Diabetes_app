using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyFoodActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyFoodActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            _petCareManager.ModifyGlycemia(-2, currentDateTime);
            return NodeState.SUCCESS;
        }
    }
}