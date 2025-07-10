using System.Collections.Generic;

namespace Master.Domain.Shop
{
    public interface IEconomyManager
    {
        public int totalCoins { get; }
        public int stashedCoins { get; }

        public void AddStashedCoins(int coins);

        public void StashedCoinsToTotalCoins();

        public void AddTotalCoins(int coins);

        public void SubstractTotalCoins(int coins);

        public Dictionary<string, ProductState> LoadAllProducts();

        public void SaveAllProducts(Dictionary<string, ProductState> allProducts);
    }
}