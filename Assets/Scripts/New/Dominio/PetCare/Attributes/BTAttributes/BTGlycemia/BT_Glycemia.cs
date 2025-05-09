using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.BehaviorTree.Activity;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class BT_Glycemia : BTree
    {
        private void Start()
        {
            SetState(TreeType.Glycemia);
            base.Start();
        }

        protected override Node SetUpTree()
        {
            #region CryticalGlucemia
            Node checkCriticalGlycemia = new Node_CheckCriticalGlycemia();
            Node applyCriticalGlycemia = new NodeGlycemia_ApplyCriticalGlycemia();

            Node cryticalGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkCriticalGlycemia,
                            applyCriticalGlycemia
                           }
                );
            #endregion
            #region LongTermButtons
            Node checkInsulineActive = new Node_CheckInsulinActive();
            Node applyInsulineActive = new NodeGlycemia_ApplyInsulinActive();
            Node checkExerciseActive = new Node_CheckExerciseActive();
            Node applyExerciseActive = new NodeGlycemia_ApplyExerciseActive();
            Node checkFoodActive = new Node_CheckFoodActive();
            Node applyFoodActive = new NodeGlycemia_ApplyFoodActive();

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
            Node checkLowActivity = new Node_CheckLowActivity();
            Node applyLowActivity = new NodeGlycemia_ApplyLowActivity();

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
            Node defaultGlycemia = new Node_DefaultGlycemia();
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