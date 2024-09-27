using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyLowActivity : Node
    {
        public NodeGlycemia_ApplyLowActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(10, currentTime);
            return NodeState.SUCCESS;
        }
    }
}