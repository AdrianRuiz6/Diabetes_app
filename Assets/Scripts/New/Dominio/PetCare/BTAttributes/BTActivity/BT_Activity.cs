using BehaviorTree;
using Master.Domain.BehaviorTree.Glycemia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Activity
{
    public class BT_Activity : BTree
    {
        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new NodeActivity_CheckExerciseActive();
            Node applyExerciseActive = new NodeActivity_ApplyExerciseActive();

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
            Node checkHighHunger = new NodeActivity_CheckHighHunger();
            Node applyHighHunger = new NodeActivity_ApplyHighHunger();

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
            Node defaultActivity = new NodeActivity_DefaultActivity();
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