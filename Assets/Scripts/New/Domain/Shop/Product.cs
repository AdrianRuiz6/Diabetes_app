using Master.Domain.GameEvents;
using Master.Presentation.Shop;
using Master.Presentation.Sound;
using Master.Persistence.Shop;
using System;
using System.Collections.Generic;

namespace Master.Domain.Shop
{
    public class Product
    {
        private string _productName;
        private int _sellingPrice;
        public ProductState productState { get; private set; }

        public Product(string name, int price)
        {
            _productName = name;
            _sellingPrice = price;

            productState = LoadThisProductState();

            if (productState == ProductState.Equipped)
            {
                GameEvents_Shop.OnProductEquippedInitialized?.Invoke(_productName);
            }
            else if (productState == ProductState.Purchased)
            {
                GameEvents_Shop.OnProductBoughtInitialized?.Invoke(_productName);
            }
        }

        public void BuyProduct()
        {
            productState = ProductState.Purchased;
            SaveThisProduct();
            EconomyManager.SubstractTotalCoins(_sellingPrice);
        }

        public void EquipProduct()
        {
            productState = ProductState.Equipped;
            SaveThisProduct();
            GameEvents_Shop.OnProductEquipped?.Invoke(_productName);
        }

        public void UnequipProduct()
        {
            productState = ProductState.Purchased;
            SaveThisProduct();
            GameEvents_Shop.OnProductEquipped?.Invoke("Base");

        }

        public void OtherProductEquipped()
        {
            productState = ProductState.Purchased;
            SaveThisProduct();
        }

        public bool IsItPurchasable()
        {
            return EconomyManager.GetTotalCoins() >= _sellingPrice;
        }

        private ProductState LoadThisProductState()
        {
            Dictionary<string, ProductState> allProducts = DataStorage_Shop.LoadProducts();
            return allProducts[_productName];
        }

        private void SaveThisProduct()
        {
            Dictionary<string, ProductState>  allProducts = DataStorage_Shop.LoadProducts();

            allProducts[_productName] = productState;

            DataStorage_Shop.SaveProducts(allProducts);
        }
    }
}