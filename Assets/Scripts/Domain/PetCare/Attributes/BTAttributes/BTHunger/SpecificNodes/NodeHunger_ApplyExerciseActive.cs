using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyExerciseActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyExerciseActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
        {
            _petCareManager.ModifyGlycemia(2, currentTime);
            return NodeState.SUCCESS;
        }
    }
}