using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsGraph
{
    public static Action<int> OnInitialTimeModified;
    public static Action<int> OnFinishTimeModified;

    public static Action<GraphFilter> OnUpdatedAttributeGraph;
    public static Action OnUpdatedActionsGraph;

    public static Action<DateTime> OnUpdatedDateGraph;
}
