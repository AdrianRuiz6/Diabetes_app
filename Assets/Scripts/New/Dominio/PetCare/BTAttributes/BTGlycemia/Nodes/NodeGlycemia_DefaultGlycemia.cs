using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Master.Domain.Events;
using System;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class NodeGlycemia_DefaultGlycemia : Node
    {
        public NodeGlycemia_DefaultGlycemia() { }

        public override NodeState Evaluate(DateTime currentTime)
        {
            Debug.LogWarning("ATRIBUTE: DEFAULT_Glycemia");  // TODO: BORRAR
            int randomGlycemia = 0;
            int randomValue = UnityEngine.Random.Range(1, 3);
            if(randomValue == 1)
            {
                randomGlycemia = -5;
            }
            else
            {
                randomGlycemia = 5;
            }
            GameEventsPetCare.OnModifyGlycemia?.Invoke(randomGlycemia, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}