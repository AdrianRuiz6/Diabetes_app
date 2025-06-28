using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class NodeEnergy_ApplyExerciseActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeEnergy_ApplyExerciseActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyEnergy(-3);
            return NodeState.SUCCESS;
        }
    }
}