using System;
using Master.Presentation.PetCare.Log;

namespace Master.Domain.GameEvents
{
    public class GameEvents_PetCareLog
    {
        public static Action<GraphFilter> OnUpdatedAttributeLog;
        public static Action OnUpdatedActionsLog;

        public static Action<DateTime> OnUpdatedDateLog;
    }
}