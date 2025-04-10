using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_DefaultHunger : Node
    {
        public NodeHunger_DefaultHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEventsPetCare.OnModifyHunger?.Invoke(2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}
