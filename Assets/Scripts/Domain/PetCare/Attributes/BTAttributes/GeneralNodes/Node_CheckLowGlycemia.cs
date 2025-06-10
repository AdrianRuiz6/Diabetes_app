using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckLowGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_CheckLowGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            if (_petCareManager.IsGlycemiaInRange(AttributeRangeValue.IntermediateLow))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}