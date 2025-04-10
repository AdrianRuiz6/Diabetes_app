using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;
using Master.Domain.Economy;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_CheckLowActivity : Node
    {
        public NodeGlycemia_CheckLowActivity() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            if (AttributeManager.Instance.IsActivityInRange(AttributeManager.Instance.activityValue, "bad1"))
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}
