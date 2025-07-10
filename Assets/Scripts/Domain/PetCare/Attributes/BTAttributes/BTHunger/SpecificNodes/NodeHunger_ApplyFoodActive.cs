using Master.Domain.BehaviorTree;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyFoodActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyFoodActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyHunger(-3);
            return NodeState.SUCCESS;
        }
    }
}