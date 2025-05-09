using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Activity
{
    public class Node_CheckExerciseActive : Node
    {
        public Node_CheckExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.isExerciseEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
