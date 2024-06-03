using BehaviorTree;
using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_DefaultHunger : Node
    {
        public NodeHunger_DefaultHunger() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(2);
            return NodeState.SUCCESS;
        }
    }
}
