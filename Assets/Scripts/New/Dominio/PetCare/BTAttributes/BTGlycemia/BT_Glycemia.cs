using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Glycemia
{
    public class BT_Glycemia : BTree
    {
        protected override Node SetUpTree()
        {
            #region CryticalGlucemia
            Node checkCriticalGlycemia = new NodeGlycemia_CheckCriticalGlycemia();
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
            Node checkInsulineActive = new NodeGlycemia_CheckInsulineActive();
            Node applyInsulineActive = new NodeGlycemia_ApplyInsulineActive();
            Node checkExerciseActive = new NodeGlycemia_CheckExerciseActive();
            Node applyExerciseActive = new NodeGlycemia_ApplyExerciseActive();
            Node checkFoodActive = new NodeGlycemia_CheckFoodActive();
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
            Node checkLowActivity = new NodeGlycemia_CheckLowActivity();
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
            Node defaultGlycemia = new NodeGlycemia_DefaultGlycemia();
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