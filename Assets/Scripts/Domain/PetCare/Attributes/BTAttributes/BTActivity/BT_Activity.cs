using System.Collections.Generic;
using Master.Domain.BehaviorTree;
using Master.Domain.Connection;
using Master.Domain.Settings;

namespace Master.Domain.PetCare
{
    public class BT_Activity : BTree
    {
        public BT_Activity(IPetCareManager petCareManager, ISettingsManager settingsManager, IConnectionManager connectionManager) : base(petCareManager, settingsManager, connectionManager)
        {
            _treeType = TreeType.Activity;
        }

        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new Node_CheckExerciseActive(_petCareManager);
            Node applyExerciseActive = new NodeActivity_ApplyExerciseActive(_petCareManager);

            Node longTermExerciseButton = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkExerciseActive,
                            applyExerciseActive
                           }
                );
            Node longTermButtons = new NodeParallel(
                new List<Node>
                           {
                            longTermExerciseButton
                           }
                );
            #endregion
            #region OtherAttributesEffects
            Node checkHighHunger = new Node_CheckHighHunger(_petCareManager);
            Node applyHighHunger = new NodeActivity_ApplyHighHunger(_petCareManager);

            Node highHunger = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkHighHunger,
                            applyHighHunger
                           }
                );
            Node otherAttributesEffects = new NodeParallel(
                new List<Node>
                           {
                            highHunger
                           }
                );
            #endregion
            #region DefaultActivity
            Node defaultActivity = new Node_DefaultActivity(_petCareManager);
            #endregion

            #region Root
            Node root =
                        new NodeSelector
                        (
                           new List<Node>
                           {
                            longTermButtons,
                            otherAttributesEffects,
                            defaultActivity
                           }
                         );
            return root;
            #endregion
        }
    }
}