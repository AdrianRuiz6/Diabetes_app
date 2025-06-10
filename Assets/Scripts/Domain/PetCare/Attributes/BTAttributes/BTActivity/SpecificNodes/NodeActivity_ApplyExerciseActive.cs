using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeActivity_ApplyExerciseActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeActivity_ApplyExerciseActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyActivity(2, currentTime);
            return NodeState.SUCCESS;
        }
    }
}