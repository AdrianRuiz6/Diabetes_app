using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeActivity_ApplyHighHunger : Node
    {
        public NodeActivity_ApplyHighHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyActivity?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}