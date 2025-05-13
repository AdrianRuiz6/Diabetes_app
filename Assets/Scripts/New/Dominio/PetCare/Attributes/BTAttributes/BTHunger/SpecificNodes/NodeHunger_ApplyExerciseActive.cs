using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyExerciseActive : Node
    {
        public NodeHunger_ApplyExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(+2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}