using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyExerciseActive : Node
    {
        public NodeGlycemia_ApplyExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(-5, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}
