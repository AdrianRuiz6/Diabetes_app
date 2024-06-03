using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Activity
{
    public class NodeActivity_CheckExerciseActive : Node
    {
        public NodeActivity_CheckExerciseActive() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.isExerciseButtonUsed)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
