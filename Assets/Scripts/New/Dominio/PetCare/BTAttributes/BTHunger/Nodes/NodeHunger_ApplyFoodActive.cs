using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_ApplyFoodActive : Node
    {
        public NodeHunger_ApplyFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(-2, currentDateTime, false);
            return NodeState.SUCCESS;
        }
    }
}