using System;


namespace Master.Domain.GameEvents
{
    public static class GameEvents_Shop
    {
        public static Action OnTotalCoinsUpdated;
        public static Action OnStashedCoinsUpdated;

        public static Action<string> OnProductEquipped;
    }
}

