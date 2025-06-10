using Master.Domain.GameEvents;
using Master.Persistence.Shop;
using System.Collections.Generic;
using System.Threading;

namespace Master.Domain.Shop
{
    public class Product : IProduct
    {
        IEconomyManager _economyManager;

        private static Mutex _mutex = new Mutex();
        private string _productName;
        private int _sellingPrice;
        public ProductState productState { get; private set; }

        public Product(string name, int price, IEconomyManager economyManager)
        {
            _economyManager = economyManager;

            _productName = name;
            _sellingPrice = price;

            productState = LoadThisProductState();
        }

        public void BuyProduct()
        {
            _mutex.WaitOne();
            try
            {
                productState = ProductState.Purchased;
                SaveThisProduct();
                _economyManager.SubstractTotalCoins(_sellingPrice);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void EquipProduct()
        {
            _mutex.WaitOne();
            try
            {
                productState = ProductState.Equipped;
                SaveThisProduct();
                GameEvents_Shop.OnProductEquipped?.Invoke(_productName);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void UnequipProduct()
        {
            _mutex.WaitOne();
            try
            {
                productState = ProductState.Purchased;
                SaveThisProduct();
                GameEvents_Shop.OnProductEquipped?.Invoke("Base");
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

        }

        public void OtherProductEquipped()
        {
            _mutex.WaitOne();
            try
            {
                productState = ProductState.Purchased;
                SaveThisProduct();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public bool IsItPurchasable()
        {
            return _economyManager.totalCoins >= _sellingPrice;
        }

        private ProductState LoadThisProductState()
        {
            Dictionary<string, ProductState> allProducts = _economyManager.LoadAllProducts();

            if (allProducts.ContainsKey(_productName)){
                return allProducts[_productName];
            }
            else
            {
                return ProductState.NotPurchased;
            }
        }

        private void SaveThisProduct()
        {
            Dictionary<string, ProductState> allProducts = _economyManager.LoadAllProducts();

            allProducts[_productName] = productState;

            _economyManager.SaveAllProducts(allProducts);
        }
    }
}