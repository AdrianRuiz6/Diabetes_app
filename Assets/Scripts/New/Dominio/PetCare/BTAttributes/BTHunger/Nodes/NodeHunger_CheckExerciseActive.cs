using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class NodeHunger_CheckExerciseActive : Node
    {
        public NodeHunger_CheckExerciseActive() { }

        public override NodeState Evaluate()
        {
            if (AttributeManager.Instance.isExerciseEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}