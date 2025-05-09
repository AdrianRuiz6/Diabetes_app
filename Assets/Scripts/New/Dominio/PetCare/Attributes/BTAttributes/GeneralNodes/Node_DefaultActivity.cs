using BehaviorTree;
using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_DefaultActivity : Node
    {
        public Node_DefaultActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyActivity?.Invoke(-2, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}