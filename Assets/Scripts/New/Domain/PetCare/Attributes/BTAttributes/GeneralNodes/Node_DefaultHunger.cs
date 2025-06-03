using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultHunger : Node
    {
        public Node_DefaultHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}
