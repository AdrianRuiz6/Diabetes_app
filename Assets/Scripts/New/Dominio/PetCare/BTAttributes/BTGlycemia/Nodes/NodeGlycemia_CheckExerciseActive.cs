using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckExerciseActive : Node
    {
        public NodeGlycemia_CheckExerciseActive() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            Debug.LogWarning("ATRIBUTE: CHECK EXERCISE");  // TODO: BORRAR
            if (AttributeManager.Instance.isExerciseEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}