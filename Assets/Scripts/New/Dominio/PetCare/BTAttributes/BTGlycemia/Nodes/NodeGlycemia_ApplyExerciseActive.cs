using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_ApplyExerciseActive : Node
    {
        public NodeGlycemia_ApplyExerciseActive() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyGlycemia?.Invoke(-5);
            return NodeState.SUCCESS;
        }
    }
}
