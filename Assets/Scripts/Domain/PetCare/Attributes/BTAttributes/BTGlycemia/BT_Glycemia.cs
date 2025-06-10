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
            #region CryticalGlucemia
            Node checkCriticalGlycemia = new Node_CheckCriticalGlycemia(_petCareManager);
            Node applyCriticalGlycemia = new NodeGlycemia_ApplyCriticalGlycemia(_petCareManager);

            Node cryticalGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkCriticalGlycemia,
                            applyCriticalGlycemia
                           }
                );
            #endregion
            #region LongTermButtons
            Node checkInsulineActive = new Node_CheckInsulinActive(_petCareManager);
            Node applyInsulineActive = new NodeGlycemia_ApplyInsulinActive(_petCareManager);
            Node checkExerciseActive = new Node_CheckExerciseActive(_petCareManager);
            Node applyExerciseActive = new NodeGlycemia_ApplyExerciseActive(_petCareManager);
            Node checkFoodActive = new Node_CheckFoodActive(_petCareManager);
            Node applyFoodActive = new NodeGlycemia_ApplyFoodActive(_petCareManager);

            Node longTermInsulineButton = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkInsulineActive,
                            applyInsulineActive
                           }
                );
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
                            longTermInsulineButton,
                            longTermExerciseButton,
                            longTermFoodButton
                           }
                );
            #endregion
            #region OtherAttributesEffects
            Node checkLowActivity = new Node_CheckLowActivity(_petCareManager);
            Node applyLowActivity = new NodeGlycemia_ApplyLowActivity(_petCareManager);

            Node lowActivity = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkLowActivity,
                            applyLowActivity
                           }
                );
            Node otherAttributesEffects = new NodeParallel(
                new List<Node>
                           {
                            lowActivity
                           }
                );
            #endregion
            #region DefaultGlycemia
            Node defaultGlycemia = new Node_DefaultGlycemia(_petCareManager);
            #endregion

            #region Root
            Node root =
                        new NodeSelector
                        (
                           new List<Node>
                           {
                            cryticalGlycemia,
                            longTermButtons,
                            otherAttributesEffects,
                            defaultGlycemia
                           }
                         );
            return root;
            #endregion
        }
    }
}