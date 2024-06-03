using BehaviorTree;
using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeActivity_DefaultActivity : Node
    {
        public NodeActivity_DefaultActivity() { }

        public override NodeState Evaluate()
        {
            GameEventsPetCare.OnModifyActivity?.Invoke(-2);
            return NodeState.SUCCESS;
        }
    }
}