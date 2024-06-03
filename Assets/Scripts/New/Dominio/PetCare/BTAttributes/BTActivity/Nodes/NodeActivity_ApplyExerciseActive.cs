using BehaviorTree;
using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Activity
{
    public class NodeActivity_ApplyExerciseActive : Node
    {
        public NodeActivity_ApplyExerciseActive() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyActivity?.Invoke(2);
            return NodeState.SUCCESS;
        }
    }
}