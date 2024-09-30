using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsGraph
{
    public static Action<int> OnInitialTimeModified;
    public static Action<int> OnFinishTimeModified;

    public static Action OnUpdatedGlycemiaGraph;
    public static Action OnUpdatedActivityGraph;
    public static Action OnUpdatedHungerGraph;

    public static Action OnUpdatedFoodGraph;
    public static Action OnUpdatedInsulinGraph;
    public static Action OnUpdatedExerciseGraph;

    public static Action<DateTime> OnUpdatedDateGraph;
}
