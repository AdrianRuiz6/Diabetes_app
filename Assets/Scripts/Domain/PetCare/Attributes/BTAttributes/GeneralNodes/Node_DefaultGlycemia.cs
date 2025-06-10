using Master.Domain.BehaviorTree;
using System;

namespace Master.Domain.PetCare
{
    public class Node_DefaultGlycemia : Node
    {
        private IPetCareManager _petCareManager;

        public Node_DefaultGlycemia(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(DateTime currentTime)
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
            _petCareManager.ModifyGlycemia(randomGlycemia, currentTime);
            return NodeState.SUCCESS;
        }
    }
}