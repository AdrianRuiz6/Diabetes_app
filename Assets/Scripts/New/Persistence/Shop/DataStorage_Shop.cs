using Master.Domain.Shop;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Master.Persistence.Shop
{
    public static class DataStorage_Shop
    {
        public static void SaveStashedCoins(int coins)
        {
            PlayerPrefs.SetInt("StashedCoins", coins);
            PlayerPrefs.Save();
        }

        public static int LoadStashedCoins()
        {
            return PlayerPrefs.GetInt("StashedCoins", 0);
        }

        public static void SaveTotalCoins(int coins)
        {
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.Save();
        }

        public static int LoadTotalCoins()
        {
            return PlayerPrefs.GetInt("Coins", 0);
        }

        public static void SaveProduct(string nameProduct, ProductState productState)
        {
            string path = $"{Application.persistentDataPath}/ProductData.txt";

            ProductDataList allProducts = new ProductDataList();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                allProducts = JsonUtility.FromJson<ProductDataList>(existingJson) ?? new ProductDataList();
            }

            ProductData existingProduct = allProducts.products.FirstOrDefault(p => p.ProductName == nameProduct);
            if (existingProduct != null)
            {
                existingProduct.ProductState = productState;
            }
            else
            {
                ProductData productData = new ProductData(nameProduct, productState);
                allProducts.products.Add(productData);
            }

            string json = JsonUtility.ToJson(allProducts, true);

            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        public static ProductState LoadProduct(string name)
        {
            string path = $"{Application.persistentDataPath}/ProductData.txt";
            if (!File.Exists(path))
                return ProductState.NotPurchased;

            string existingJson = null;

            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            ProductDataList allProducts = JsonUtility.FromJson<ProductDataList>(existingJson);

            ProductData result = allProducts.products.FirstOrDefault(product => product.ProductName == name);
            if (result == null)
            {
                return ProductState.NotPurchased;
            }

            return result.ProductState;
        }
    }
}