using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyExerciseActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeGlycemia_ApplyExerciseActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyGlycemia(-5, currentTime);
            return NodeState.SUCCESS;
        }
    }
}
