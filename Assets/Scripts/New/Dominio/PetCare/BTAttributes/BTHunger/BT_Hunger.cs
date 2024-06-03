using BehaviorTree;
using Master.Domain.BehaviorTree.Glycemia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.BehaviorTree.Hunger
{
    public class BT_Hunger : BTree
    {
        protected override Node SetUpTree()
        {
            #region LongTermButtons
            Node checkExerciseActive = new NodeHunger_CheckExerciseActive();
            Node applyExerciseActive = new NodeHunger_ApplyExerciseActive();
            Node checkFoodActive = new NodeHunger_CheckFoodActive();
            Node applyFoodActive = new NodeHunger_ApplyExerciseActive();

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
            Node checkLowGlycemia = new NodeHunger_CheckLowGlycemia();
            Node applyLowGlycemia = new NodeHunger_ApplyLowGlycemia();
            Node checkHighGlycemia = new NodeHunger_CheckHighGlycemia();
            Node applyHighGlycemia = new NodeHunger_ApplyHighGlycemia();

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
            Node defaultHunger = new NodeHunger_DefaultHunger();
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