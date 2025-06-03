using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyFoodActive : Node
    {
        public NodeGlycemia_ApplyFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(+5, currentDateTime, false);
            return NodeState.SUCCESS;
        }
    }
}