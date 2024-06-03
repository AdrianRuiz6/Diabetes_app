using BehaviorTree;
using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_ApplyLowGlycemia : Node
    {
        public NodeHunger_ApplyLowGlycemia() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(+5);
            return NodeState.SUCCESS;
        }
    }
}