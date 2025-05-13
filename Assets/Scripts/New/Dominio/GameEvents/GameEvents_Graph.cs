using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Master.Presentation.Graph;

namespace Master.Domain.GameEvents
{
    public class GameEvents_Graph
    {
        public static Action<int> OnInitialTimeModified;
        public static Action<int> OnFinishTimeModified;

        public static Action<GraphFilter> OnUpdatedAttributeGraph;
        public static Action OnUpdatedActionsGraph;

        public static Action<DateTime> OnUpdatedDateGraph;
    }
}