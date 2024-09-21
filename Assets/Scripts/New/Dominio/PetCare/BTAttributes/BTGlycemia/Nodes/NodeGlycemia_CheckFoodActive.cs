using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckFoodActive : Node
    {
        public NodeGlycemia_CheckFoodActive() { }

        public override NodeState Evaluate()
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
