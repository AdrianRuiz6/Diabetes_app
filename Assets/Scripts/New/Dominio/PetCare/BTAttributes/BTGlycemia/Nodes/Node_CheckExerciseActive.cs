using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckExerciseActive : Node
    {
        public Node_CheckExerciseActive() { }

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