using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class Node_RandomGlycemia : Node
    {
        public Node_RandomGlycemia() { }

        public override NodeState Evaluate()
        {
            int randomGlycemia = 0;
            int randomValue = Random.Range(1, 3);
            if(randomValue == 1)
            {
                randomGlycemia = -5;
            }
            else
            {
                randomGlycemia = 5;
            }

            AttributeManager.Instance.ModifyGlycemia(randomGlycemia);
            return NodeState.SUCCESS;
        }
    }
}