using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_CheckExerciseActive : Node
    {
        public Node_CheckExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.isExerciseEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
