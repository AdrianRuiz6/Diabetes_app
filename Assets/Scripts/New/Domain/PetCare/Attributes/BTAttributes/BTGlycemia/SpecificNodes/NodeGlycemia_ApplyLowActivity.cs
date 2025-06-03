using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class NodeGlycemia_ApplyLowActivity : Node
    {
        public NodeGlycemia_ApplyLowActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(10, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}