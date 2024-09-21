using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckExerciseActive : Node
    {
        public NodeGlycemia_CheckExerciseActive() { }

        public override NodeState Evaluate()
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