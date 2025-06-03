using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyExerciseActive : Node
    {
        public NodeGlycemia_ApplyExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}
