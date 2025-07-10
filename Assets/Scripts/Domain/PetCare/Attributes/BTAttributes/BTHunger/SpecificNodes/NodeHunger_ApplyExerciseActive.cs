using Master.Domain.BehaviorTree;

namespace Master.Domain.PetCare
{
    public class NodeHunger_ApplyExerciseActive : Node
    {
        private IPetCareManager _petCareManager;

        public NodeHunger_ApplyExerciseActive(IPetCareManager petCareManager)
        {
            _petCareManager = petCareManager;
        }

        public override NodeState Evaluate(AttributeUpdateIntervalInfo intervalInfo)
        {
            _petCareManager.ModifyHunger(2);
            return NodeState.SUCCESS;
        }
    }
}