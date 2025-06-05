using Master.Domain.GameEvents;
using Master.Persistence.Shop;

namespace Master.Domain.Shop
{
    public class EconomyManager
    {
        private int _totalCoins;
        private int _stashedCoins;

        public EconomyManager()
        {
            _stashedCoins = DataStorage_Shop.LoadStashedCoins();
            GameEvents_Shop.OnStashedCoinsUpdated?.Invoke(_stashedCoins);

            _totalCoins = DataStorage_Shop.LoadTotalCoins();
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);

            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddStashedCoins(int coins)
        {
            SetStashedCoins(_stashedCoins + coins);
            GameEvents_Shop.OnStashedCoinsUpdated?.Invoke(_stashedCoins);
        }

        public void StashedCoinsToTotalCoins()
        {
            SetTotalCoins(_totalCoins + _stashedCoins);
            SetStashedCoins(0);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void AddTotalCoins(int coins)
        {
            SetTotalCoins(_totalCoins + coins);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public void SubstractTotalCoins(int coins)
        {
            SetTotalCoins(_totalCoins - coins);
            GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(_totalCoins);
        }

        public int GetTotalCoins()
        {
            return _totalCoins;
        }

        private void SetTotalCoins(int newTotalCoins)
        {
            _totalCoins = newTotalCoins;
            DataStorage_Shop.SaveTotalCoins(_totalCoins);
        }

        private void SetStashedCoins(int newStashedCoins)
        {
            _stashedCoins = newStashedCoins;
            DataStorage_Shop.SaveStashedCoins(_stashedCoins);
        }
    }
}