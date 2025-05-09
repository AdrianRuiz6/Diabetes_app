using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_CheckInsulinActive : Node
    {
        public Node_CheckInsulinActive() { }

        public override NodeState Evaluate(DateTime currentTime)
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
