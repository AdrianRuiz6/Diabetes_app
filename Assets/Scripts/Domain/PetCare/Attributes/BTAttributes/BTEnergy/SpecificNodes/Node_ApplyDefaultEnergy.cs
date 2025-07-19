using Master.Domain.BehaviorTree;

namespace Master.Domain.PetCare
{
    public class Node_ApplyDefaultEnergy : Node
    {
        private IPetCareManager _petCareManager;

        public Node_ApplyDefaultEnergy(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyEnergy(-2);
            return NodeState.SUCCESS;
        }
    }
}