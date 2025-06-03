using UnityEngine;
using Master.Domain.BehaviorTree;
using Master.Domain.GameEvents;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultGlycemia : Node
    {
        public Node_DefaultGlycemia() { }

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
            GameEvents_PetCare.OnModifyGlycemia?.Invoke(randomGlycemia, currentTime, false);
            return NodeState.SUCCESS;
        }
    }
}