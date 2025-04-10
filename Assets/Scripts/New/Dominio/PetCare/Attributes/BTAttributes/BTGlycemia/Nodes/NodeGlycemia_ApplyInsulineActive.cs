using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyInsulineActive : Node
    {
        public NodeGlycemia_ApplyInsulineActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}