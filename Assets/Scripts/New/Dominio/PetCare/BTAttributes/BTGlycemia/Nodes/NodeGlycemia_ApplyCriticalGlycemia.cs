using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyCriticalGlycemia : Node
    {
        public NodeGlycemia_ApplyCriticalGlycemia() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(20);
            return NodeState.SUCCESS;
        }
    }
}