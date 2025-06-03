using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyHighGlycemia : Node
    {
        public NodeHunger_ApplyHighGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}