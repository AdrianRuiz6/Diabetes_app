using Master.Domain.PetCare;
using System;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_PetCare
    {
        public static Action<int> OnModifyGlycemiaUI;
        public static Action<int> OnModifyActivityUI;
        public static Action<int> OnModifyHungerUI;

        public static Action<ActionType, float> OnStartTimerCD;
        public static Action<ActionType> OnFinishTimerCD;

        public static Action<DateTime> OnExecuteAttributesBTree;
    }
}