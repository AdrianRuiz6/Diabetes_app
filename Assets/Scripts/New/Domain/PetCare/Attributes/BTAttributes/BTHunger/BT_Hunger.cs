using Master.Domain.BehaviorTree;
using System.Collections.Generic;


namespace Master.Domain.PetCare
{
    public class BT_Hunger : BTree
    {
        private void Start()
        {
            SetState(TreeType.Hunger);
            base.Start();
        }

        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new Node_CheckExerciseActive();
            Node applyExerciseActive = new NodeHunger_ApplyExerciseActive();
            Node checkFoodActive = new Node_CheckFoodActive();
            Node applyFoodActive = new NodeHunger_ApplyExerciseActive();

            Node longTermExerciseButton = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkExerciseActive,
                            applyExerciseActive
                           }
                );
            Node longTermFoodButton = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkFoodActive,
                            applyFoodActive
                           }
                );
            Node longTermButtons = new NodeParallel(
                new List<Node>
                           {
                            longTermExerciseButton,
                            longTermFoodButton
                           }
                );
            #endregion
            #region OtherAttributesEffects
            Node checkLowGlycemia = new Node_CheckLowGlycemia();
            Node applyLowGlycemia = new NodeHunger_ApplyLowGlycemia();
            Node checkHighGlycemia = new Node_CheckHighGlycemia();
            Node applyHighGlycemia = new NodeHunger_ApplyHighGlycemia();

            Node lowGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkLowGlycemia,
                            applyLowGlycemia
                           }
                );
            Node highGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkHighGlycemia,
                            applyHighGlycemia
                           }
                );
            Node otherAttributesEffects = new NodeParallel(
                new List<Node>
                           {
                            lowGlycemia,
                            highGlycemia
                           }
                );
            #endregion
            #region DefaultGlycemia
            Node defaultHunger = new Node_DefaultHunger();
            #endregion

            #region Root
            Node root =
                        new NodeSelector
                        (
                           new List<Node>
                           {
                            longTermButtons,
                            otherAttributesEffects,
                            defaultHunger
                           }
                         );
            return root;
            #endregion
        }
    }
}