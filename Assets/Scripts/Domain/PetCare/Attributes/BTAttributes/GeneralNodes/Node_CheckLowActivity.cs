using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckLowActivity : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckLowActivity(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (_petCareManager.IsActivityInRange(AttributeRangeValue.BadLow))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
