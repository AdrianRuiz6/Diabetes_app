using System;


namespace Master.Domain.Events
{
    public static class GameEventsEconomy
    {
        public static Action<int> OnTotalCoinsUpdated;
        public static Action<int> OnStashedCoinsUpdated;
        public static Action<string> OnProductEquipped;
        public static Action<string> OnProductBought;
        public static Action<string> OnNotEnoughMoney;
        public static Action<string> OnEnoughMoney;
    }
}

