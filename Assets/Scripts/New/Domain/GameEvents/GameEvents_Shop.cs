using System;


namespace Master.Domain.GameEvents
{
    public static class GameEvents_Shop
    {
        public static Action<int> OnTotalCoinsUpdated;
        public static Action<int> OnStashedCoinsUpdated;

        public static Action<string> OnProductEquipped;
    }
}

