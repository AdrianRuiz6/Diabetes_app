using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckInsulineActive : Node
    {
        public NodeGlycemia_CheckInsulineActive() { }

        public override NodeState Evaluate()
        {
            Debug.LogWarning("ATRIBUTE: CHECK INSULINE");  // TODO: BORRAR
            if (AttributeManager.Instance.isInsulinEffectActive)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
