using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckFoodActive : Node
    {
        public NodeGlycemia_CheckFoodActive() { }

        public override NodeState Evaluate(DateTime currentDateTime)
        {
            Debug.LogWarning("ATRIBUTE: CHECK FOOD");  // TODO: BORRAR
            if (AttributeManager.Instance.isFoodEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
