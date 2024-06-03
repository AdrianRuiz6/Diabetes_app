using BehaviorTree;
using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_ApplyHighGlycemia : Node
    {
        public NodeHunger_ApplyHighGlycemia() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(-5);
            return NodeState.SUCCESS;
        }
    }
}