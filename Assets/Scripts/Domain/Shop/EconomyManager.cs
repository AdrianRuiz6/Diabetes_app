using Master.Domain.GameEvents;
using Master.Persistence.Shop;
using System.Collections.Generic;
using System.Threading;

namespace Master.Domain.Shop
{
    public class EconomyManager : IEconomyManager
    {
        IShopRepository _shopRepository;
        private Mutex _mutex = new Mutex();
        public int totalCoins { get; private set; }
        public int stashedCoins { get; private set; }

        public EconomyManager(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;

            stashedCoins = _shopRepository.LoadStashedCoins();
            totalCoins = _shopRepository.LoadTotalCoins();
        }

        public void AddStashedCoins(int coins)
        {
            _mutex.WaitOne();
            try
            {
                SetStashedCoins(stashedCoins + coins);
                GameEvents_Shop.OnStashedCoinsUpdated?.Invoke(stashedCoins);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void StashedCoinsToTotalCoins()
        {
            _mutex.WaitOne();
            try
            {
                SetTotalCoins(totalCoins + stashedCoins);
                SetStashedCoins(0);
                GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void AddTotalCoins(int coins)
        {
            _mutex.WaitOne();
            try
            {
                SetTotalCoins(totalCoins + coins);
                GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void SubstractTotalCoins(int coins)
        {
            _mutex.WaitOne();
            try
            {
                SetTotalCoins(totalCoins - coins);
                GameEvents_Shop.OnTotalCoinsUpdated?.Invoke(totalCoins);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private void SetTotalCoins(int newTotalCoins)
        {
            totalCoins = newTotalCoins;
            _shopRepository.SaveTotalCoins(totalCoins);
        }

        private void SetStashedCoins(int newStashedCoins)
        {
            stashedCoins = newStashedCoins;
            _shopRepository.SaveStashedCoins(stashedCoins);
        }

        public Dictionary<string, ProductState> LoadAllProducts()
        {
            return _shopRepository.LoadProducts();
        }

        public void SaveAllProducts(Dictionary<string, ProductState> allProducts)
        {
            _shopRepository.SaveProducts(allProducts);
        }
    }
}