using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_ApplyHighGlycemia : Node
    {
        public NodeHunger_ApplyHighGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}