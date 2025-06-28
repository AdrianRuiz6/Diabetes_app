using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.Settings;
using System.Collections.Generic;

namespace Master.Domain.PetCare
{
    public class BT_Glycemia : BTree
    {
        public BT_Glycemia(IPetCareManager petCareManager, ISettingsManager settingsManager, IConnectionManager connectionManager) : base(petCareManager, settingsManager, connectionManager)
        {
            _treeType = TreeType.Glycemia;
        }

        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new Node_CheckExerciseActive(_petCareManager);
            Node applyExerciseActive = new NodeGlycemia_ApplyExerciseActive(_petCareManager);
            Node checkFoodActive = new Node_CheckFoodActive(_petCareManager);
            Node applyFoodActive = new NodeGlycemia_ApplyFoodActive(_petCareManager);
            Node checkInsulinActivealGlycemia = new Node_CheckInsulinActive(_petCareManager);
            Node applyInsulinActive = new NodeGlycemia_ApplyInsulinActive(_petCareManager);

            Node longTermInsulineAction = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkInsulinActivealGlycemia,
                            applyInsulinActive
                           }
                );
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

            Node longTermActions = new NodeParallel(
                new List<Node>
                           {
                            longTermInsulineAction,
                            longTermExerciseAction,
                            longTermFoodAction
                           }
                );
            #endregion
            #region OtherAttributesEffects
            Node checkIntermediateHighHunger = new Node_CheckIntermediateHighEnergy(_petCareManager);
            Node applyIntermediateHighHunger = new NodeGlycemia_ApplyIntermediateHighEnergy(_petCareManager);
            Node checkBadHighHunger = new Node_CheckBadHighEnergy(_petCareManager);
            Node applyBadHighHunger = new NodeGlycemia_ApplyBadHighEnergy(_petCareManager);

            Node intermediateHighHunger = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkIntermediateHighHunger,
                            applyIntermediateHighHunger
                           }
                );
            Node badHighHunger = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkBadHighHunger,
                            applyBadHighHunger
                           }
                );

            Node highHunger = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            intermediateHighHunger,
                            badHighHunger
                           }
                );

            Node otherAttributesEffects = new NodeParallel(
                new List<Node>
                           {
                            highHunger
                           }
                );
            #endregion
            #region DefaultGlycemia
            Node defaultGlycemia = new Node_ApplyDefaultGlycemia(_petCareManager);
            #endregion

            #region Root
            Node root =
                        new NodeSelector
                        (
                           new List<Node>
                           {
                            longTermActions,
                            otherAttributesEffects,
                            defaultGlycemia
                           }
                         );
            return root;
            #endregion
        }
    }
}