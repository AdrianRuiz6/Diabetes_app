using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_ApplyDefaultGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_ApplyDefaultGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
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

            _petCareManager.ModifyGlycemia(randomGlycemia);
            return NodeState.SUCCESS;
        }
    }
}