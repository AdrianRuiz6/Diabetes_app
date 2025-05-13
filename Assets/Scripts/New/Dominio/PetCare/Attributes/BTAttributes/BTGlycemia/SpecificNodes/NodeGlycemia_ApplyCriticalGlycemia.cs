using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyCriticalGlycemia : Node
    {
        public NodeGlycemia_ApplyCriticalGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(20, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}