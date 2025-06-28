using System.Collections.Generic;
using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.Settings;

namespace Master.Domain.PetCare
{
    public class BT_Energy : BTree
    {
        public BT_Energy(IPetCareManager petCareManager, ISettingsManager settingsManager, IConnectionManager connectionManager) : base(petCareManager, settingsManager, connectionManager)
        {
            _treeType = TreeType.Energy;
        }

        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new Node_CheckExerciseActive(_petCareManager);
            Node applyExerciseActive = new NodeEnergy_ApplyExerciseActive(_petCareManager);
            Node checkFoodActive = new Node_CheckFoodActive(_petCareManager);
            Node applyFoodActive = new NodeEnergy_ApplyFoodActive(_petCareManager);

            Node longTermExerciseAction = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkExerciseActive,
                            applyExerciseActive
                           }
                );
            Node longTermFoodAction = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkFoodActive,
                            applyFoodActive
                           }
                );

            Node longTermButtons = new NodeParallel(
                new List<Node>
                           {
                            longTermExerciseAction,
                            longTermFoodAction
                           }
                );
            #endregion
            #region OtherAttributesEffects
            Node checkIntermediateLowGlycemia = new Node_CheckIntermediateLowGlycemia(_petCareManager);
            Node checkBadLowGlycemia = new Node_CheckBadLowGlycemia(_petCareManager);
            Node applyIntermediateLowGlycemia = new NodeEnergy_ApplyIntermediateLowGlycemia(_petCareManager);
            Node applyBadLowGlycemia = new NodeEnergy_ApplyBadLowGlycemia(_petCareManager);

            Node intermediateLowGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkIntermediateLowGlycemia,
                            applyIntermediateLowGlycemia
                           }
                );
            Node badLowGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkBadLowGlycemia,
                            applyBadLowGlycemia
                           }
                );

            Node lowGlycemia = new NodeSelector(
                new List<Node>
                           {
                            intermediateLowGlycemia,
                            badLowGlycemia
                           }
                );

            Node otherAttributesEffects = new NodeParallel(
                new List<Node>
                           {
                            lowGlycemia
                           }
                );
            #endregion
            #region DefaultActivity
            Node defaultenergy = new Node_ApplyDefaultEnergy(_petCareManager);
            #endregion

            #region Root
            Node root =
                        new NodeSelector
                        (
                           new List<Node>
                           {
                            longTermButtons,
                            otherAttributesEffects,
                            defaultenergy
                           }
                         );
            return root;
            #endregion
        }
    }
}