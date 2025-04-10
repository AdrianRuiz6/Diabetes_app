using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckCriticalGlycemia : Node
    {
        public NodeGlycemia_CheckCriticalGlycemia(){}

        public override NodeState Evaluate(DateTime currentTime)
        {
            Debug.LogWarning("ATRIBUTE: CHECK CRITICAL GLYCEMIA");  // TODO: BORRAR
            if (AttributeManager.Instance.glycemiaValue <= 40)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}