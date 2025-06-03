using System;

namespace Master.Domain.GameEvents
{
    public class GameEvents_Settings
    {
        public static Action<int> OnInitialTimeModified;
        public static Action<int> OnFinishTimeModified;
    }
}