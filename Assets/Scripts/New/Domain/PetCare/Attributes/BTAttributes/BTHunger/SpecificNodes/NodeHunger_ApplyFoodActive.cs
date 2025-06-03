using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyFoodActive : Node
    {
        public NodeHunger_ApplyFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(-2, currentDateTime, false);
            return NodeState.SUCCESS;
        }
    }
}