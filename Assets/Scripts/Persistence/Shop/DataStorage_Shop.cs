using Master.Domain.Shop;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Master.Persistence.Shop
{
    public class DataStorage_Shop : IShopRepository
    {
        public void SaveStashedCoins(int coins)
        {
            PlayerPrefs.SetInt("StashedCoins", coins);
            PlayerPrefs.Save();
        }

        public int LoadStashedCoins()
        {
            return PlayerPrefs.GetInt("StashedCoins", 0);
        }

        public void SaveTotalCoins(int coins)
        {
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.Save();
        }

        public int LoadTotalCoins()
        {
            return PlayerPrefs.GetInt("Coins", 0);
        }

        public void SaveProducts(Dictionary<string, ProductState> newAllProducts)
        {
            string path = $"{Application.persistentDataPath}/ProductData.txt";

            ProductDataList allProducts = new ProductDataList();

            foreach (var entry in newAllProducts)
            {
                allProducts.products.Add(new ProductData(entry.Key, entry.Value));
            }

            string json = JsonUtility.ToJson(allProducts, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        public Dictionary<string, ProductState> LoadProducts()
        {
            string path = $"{Application.persistentDataPath}/ProductData.txt";
            Dictionary<string, ProductState> result = new Dictionary<string, ProductState>();

            if (!File.Exists(path))
                return result;

            string existingJson = null;
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }
            ProductDataList allProducts = JsonUtility.FromJson<ProductDataList>(existingJson);

            if (allProducts != null && allProducts.products != null)
            {
                foreach (ProductData productData in allProducts.products)
                {
                    result[productData.ProductName] = productData.ProductState;
                }
            }

            return result;
        }
    }
}