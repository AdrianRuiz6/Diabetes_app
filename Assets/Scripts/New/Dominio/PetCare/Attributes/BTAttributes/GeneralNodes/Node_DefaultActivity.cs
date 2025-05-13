using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultActivity : Node
    {
        public Node_DefaultActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyActivity?.Invoke(-2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}