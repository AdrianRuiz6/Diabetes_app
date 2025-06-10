using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultActivity : Node
    {
        private IPetCareManager _petCareManager;

        public Node_DefaultActivity(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyActivity(-2, currentTime);
            return NodeState.SUCCESS;
        }
    }
}