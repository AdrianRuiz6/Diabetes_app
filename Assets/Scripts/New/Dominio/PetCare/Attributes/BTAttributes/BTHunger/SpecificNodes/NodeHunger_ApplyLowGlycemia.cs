using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyLowGlycemia : Node
    {
        public NodeHunger_ApplyLowGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(+5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}