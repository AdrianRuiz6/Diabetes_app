using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Domain.BehaviorTree.Glycemia;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class BT_Glycemia : BTree
    {
        protected override Node SetUpTree()
        {
            #region CryticalGlucemia
            Node checkCriticalGlycemia = new Node_CheckCriticalGlycemia();
            Node applyCriticalGlycemia = new Node_ApplyCriticalGlycemia();

            Node cryticalGlycemia = new NodeSequenceLeftRight(
                new List<Node>
                           {
                            checkCriticalGlycemia,
                            applyCriticalGlycemia
                           }
                );
            #endregion
            #region LongTermButtons
            Node checkInsulineActive = new Node_CheckInsulineActive();
            Node applyInsulineActive = new Node_ApplyInsulineActive();
            Node checkExerciseActive = new Node_CheckExerciseActive();
            Node applyExerciseActive = new Node_ApplyExerciseActive();
            Node checkFoodActive = new Node_CheckFoodActive();
            Node applyFoodActive = new Node_ApplyFoodActive();

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
            Node applyLowActivity = new Node_ApplyLowActivity();

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
                            longTermInsulineButton,
                            longTermExerciseButton,
                            longTermFoodButton
                           }
                );
            #endregion
            #region RandomGlycemia
            Node randomGlycemia = new Node_RandomGlycemia();
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
                            randomGlycemia
                           }
                         );
            return root;
            #endregion
        }
    }
}