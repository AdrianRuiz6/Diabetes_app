using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyFoodActive : Node
    {
        public NodeGlycemia_ApplyFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(+5, currentDateTime);
            return NodeState.SUCCESS;
        }
    }
}