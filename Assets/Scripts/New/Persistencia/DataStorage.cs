using Master.Domain.Economy;
using Master.Domain.States;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class DataStorage
{
    public static void SaveCoins(int coins)
    {
        string path = $"{Application.persistentDataPath}/CoinsData.txt";
        CoinsData coinsData = new CoinsData(coins);
        string json = JsonUtility.ToJson(coinsData);

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }

    public static int LoadCoins()
    {
        string path = $"{Application.persistentDataPath}/CoinsData.txt";
        CoinsData result = new CoinsData();
        if (!File.Exists(path))
            return result.Coins;

        string jsonFromFile = null;
        if (File.Exists(path))
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                jsonFromFile = streamReader.ReadToEnd();
                result = JsonUtility.FromJson<CoinsData>(jsonFromFile);
            }
        }

        return result.Coins;
    }

    public static void SaveProduct(string nameProduct, ProductState productState)
    {
        string path = $"{Application.persistentDataPath}/ProductData.txt";
        ProductData productData = new ProductData(nameProduct, productState);
        string json = JsonUtility.ToJson(productData);

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }


    public static void SaveProduct_____(string nameProduct, ProductState productState)
    {
        string path = $"{Application.persistentDataPath}/ProductData.txt";
        
        // Lista donde almacenar todos los productos ya incluidos en el json.
        ProductDataList allProducts = new ProductDataList();
        allProducts.products = new List<ProductData>();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            ProductDataList loadedProducts = JsonUtility.FromJson<ProductDataList>(existingJson);

            if(loadedProducts != null && loadedProducts.products != null)
            {
                allProducts.products = loadedProducts.products;
            }
        }

        ProductData productData = new ProductData(nameProduct, productState);
        allProducts.products.Add(productData);

        string json = JsonUtility.ToJson(allProducts);

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

        string jsonFromFile = null;
        if (File.Exists(path))
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                jsonFromFile = streamReader.ReadToEnd();
            }
        }

        // Deserializa json en una lista de ProductData.
        List<ProductData> allProducts = JsonUtility.FromJson<List<ProductData>>(jsonFromFile);
        if(allProducts == null)
        {
            return ProductState.NotPurchased;
        }

        // Encuentra el ProductData solicitado.
        ProductData result = allProducts.FirstOrDefault(product => product.ProductName == name);
        if(result == null)
        {
            return ProductState.NotPurchased;
        }


        return result.ProductState;
    }

    public static void SaveUserPerformance(Dictionary<string, FixedSizeQueue<char>> userPerformance) //TODO
    {

    }

    public static Dictionary<string, FixedSizeQueue<char>> LoadUserPerformance() // TODO
    {
        return LoadUserPerformance();
    }

    public static List<Question> LoadQuestions() // TODO: return null si no puede cargar
    {
        return LoadQuestions();
    }
}
