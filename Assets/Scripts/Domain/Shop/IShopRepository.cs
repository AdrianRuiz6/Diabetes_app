using System.Collections.Generic;

namespace Master.Domain.Shop
{
    public interface IShopRepository
    {
        public void SaveStashedCoins(int coins);
        public int LoadStashedCoins();

        public void SaveTotalCoins(int coins);

        public int LoadTotalCoins();

        public void SaveProducts(Dictionary<string, ProductState> newAllProducts);

        public Dictionary<string, ProductState> LoadProducts();
    }
}