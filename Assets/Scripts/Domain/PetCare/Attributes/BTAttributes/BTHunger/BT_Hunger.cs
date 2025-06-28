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
            Node checkIntermediateLowEnergy = new Node_CheckIntermediateLowEnergy(_petCareManager);
            Node checkBadLowEnergy = new Node_CheckBadLowEnergy(_petCareManager);
            Node applyIntermediateLowEnergy = new NodeHunger_ApplyIntermediateLowEnergy(_petCareManager);
            Node applyBadLowEnergy = new NodeHunger_ApplyBadLowEnergy(_petCareManager);
            Node checkIntermediateLowGlycemia = new Node_CheckIntermediateLowGlycemia(_petCareManager);
            Node checkBadLowGlycemia = new Node_CheckBadLowGlycemia(_petCareManager);
            Node applyIntermediateLowGlycemia = new NodeHunger_ApplyIntermediateLowGlycemia(_petCareManager);
            Node applyBadLowGlycemia = new NodeHunger_ApplyBadLowGlycemia(_petCareManager);

            Node intermediateLowEnergy = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkIntermediateLowEnergy,
                            applyIntermediateLowEnergy
                           }
                );
            Node badLowEnergy = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkBadLowEnergy,
                            applyBadLowEnergy
                           }
                );
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

            Node lowEnergy = new NodeSelector(
                new List<Node>
                           {
                            intermediateLowEnergy,
                            badLowEnergy
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
                            lowGlycemia,
                            lowEnergy
                           }
                );
            #endregion
            #region DefaultGlycemia
            Node defaultHunger = new Node_ApplyDefaultHunger(_petCareManager);
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