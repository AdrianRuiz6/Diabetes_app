using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.Settings;
using System.Collections.Generic;


namespace Master.Domain.PetCare
{
    public class BT_Hunger : BTree
    {
        public BT_Hunger(IPetCareManager petCareManager, ISettingsManager settingsManager, IConnectionManager connectionManager) : base(petCareManager, settingsManager, connectionManager)
        {
            _treeType = TreeType.Hunger;
        }

        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new Node_CheckExerciseActive(_petCareManager);
            Node applyExerciseActive = new NodeHunger_ApplyExerciseActive(_petCareManager);
            Node checkFoodActive = new Node_CheckFoodActive(_petCareManager);
            Node applyFoodActive = new NodeHunger_ApplyExerciseActive(_petCareManager);

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
            Node checkLowGlycemia = new Node_CheckLowGlycemia(_petCareManager);
            Node applyLowGlycemia = new NodeHunger_ApplyLowGlycemia(_petCareManager);
            Node checkHighGlycemia = new Node_CheckHighGlycemia(_petCareManager);
            Node applyHighGlycemia = new NodeHunger_ApplyHighGlycemia(_petCareManager);

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
            Node defaultHunger = new Node_DefaultHunger(_petCareManager);
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