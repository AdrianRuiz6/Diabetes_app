using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class Node_DefaultHunger : Node
    {
        public Node_DefaultHunger() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyHunger?.Invoke(2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}
