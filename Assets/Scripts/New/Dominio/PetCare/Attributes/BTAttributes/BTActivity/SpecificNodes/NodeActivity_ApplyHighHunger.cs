using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeActivity_ApplyHighHunger : Node
    {
        public NodeActivity_ApplyHighHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyActivity?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}