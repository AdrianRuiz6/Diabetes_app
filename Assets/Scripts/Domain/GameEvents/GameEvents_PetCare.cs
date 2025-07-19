using System;
using Master.Domain.PetCare;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_PetCare
    {
        public static Action<int> OnModifyGlycemiaUI;
        public static Action<int> OnModifyEnergyUI;
        public static Action<int> OnModifyHungerUI;

        public static Action<ActionType> OnStartTimerCD;
        public static Action<ActionType> OnFinishTimerCD;

        public static Action<AttributeUpdateIntervalInfo> OnExecuteAttributesBTree;
        public static Action OnFinishedExecutionAttributesBTree;

        public static Action OnFinishedSimulation;
    }
}