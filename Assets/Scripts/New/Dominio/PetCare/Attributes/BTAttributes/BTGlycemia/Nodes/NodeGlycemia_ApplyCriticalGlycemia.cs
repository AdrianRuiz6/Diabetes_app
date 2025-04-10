using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyCriticalGlycemia : Node
    {
        public NodeGlycemia_ApplyCriticalGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(20, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}