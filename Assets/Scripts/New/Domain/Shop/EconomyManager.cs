using Master.Domain.GameEvents;
using Master.Persistence.Shop;

namespace Master.Domain.Shop
{
    public class EconomyManager
    {
        public int totalCoins { get; private set; }
        public int stashedCoins { get; private set; }

        public EconomyManager()
        {
            stashedCoins = DataStorage_Shop.LoadStashedCoins();
            totalCoins = DataStorage_Shop.LoadTotalCoins();
        }

        public void AddStashedCoins(int coins)
        {
            SetStashedCoins(stashedCoins + coins);
            GameEvents_Shop.OnStashedCoinsUpdated?.Invoke(stashedCoins);
        }

        public void StashedCoinsToTotalCoins()
        {
            SetTotalCoins(totalCoins + stashedCoins);
            SetStashedCoins(0);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
        }

        public void AddTotalCoins(int coins)
        {
            SetTotalCoins(totalCoins + coins);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
        }

        public void SubstractTotalCoins(int coins)
        {
            SetTotalCoins(totalCoins - coins);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
        }

        private void SetTotalCoins(int newTotalCoins)
        {
            totalCoins = newTotalCoins;
            DataStorage_Shop.SaveTotalCoins(totalCoins);
        }

        private void SetStashedCoins(int newStashedCoins)
        {
            stashedCoins = newStashedCoins;
            DataStorage_Shop.SaveStashedCoins(stashedCoins);
        }
    }
}