using Master.Domain.Economy;
using Master.Domain.States;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

        string existingJson = null;
        if (File.Exists(path))
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
                result = JsonUtility.FromJson<CoinsData>(existingJson);
            }
        }

        return result.Coins;
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

    public static void SaveUserPerformance(Dictionary<string, FixedSizeQueue<char>> userPerformance)
    {
        string path = $"{Application.persistentDataPath}/UserPerformanceData.txt";

        UserPerformanceDataList dataList = new UserPerformanceDataList();

        foreach (var kvp in userPerformance)
        {
            UserPerformanceData data = new UserPerformanceData(kvp.Key, kvp.Value.ToList());
            dataList.userPerformanceList.Add(data);
        }

        string json = JsonUtility.ToJson(dataList);

        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }


    public static Dictionary<string, FixedSizeQueue<char>> LoadUserPerformance(List<string> allTopics)
    {
        string path = $"{Application.persistentDataPath}/UserPerformanceData.txt";
        Dictionary<string, FixedSizeQueue<char>> userPerformance = new Dictionary<string, FixedSizeQueue<char>>();

        if (!File.Exists(path))
        {
            foreach (string topic in allTopics)
            {
                userPerformance.Add(topic, new FixedSizeQueue<char>());
            }

            return userPerformance;
        }

        string existingJson = null;
        using (StreamReader streamReader = new StreamReader(path))
        {
            existingJson = streamReader.ReadToEnd();
        }

        UserPerformanceDataList dataList = JsonUtility.FromJson<UserPerformanceDataList>(existingJson);

        foreach (var data in dataList.userPerformanceList)
        {
            FixedSizeQueue<char> queue = new FixedSizeQueue<char>(data.performanceData);

            userPerformance.Add(data.topic, queue);
        }

        foreach (string topic in allTopics)
        {
            if(userPerformance.ContainsKey(topic) == false)
            {
                userPerformance.Add(topic, new FixedSizeQueue<char>());
            }
        }

        return userPerformance;
    }


    public static List<Question> LoadQuestions()
    {
        string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTmL6z_aRoGzC_IM7f85ga5WoVNv99d4oUYIsjTrLzUgKLUuzc4xXIY8n_TakI-OQ/pub?gid=180804845&single=true&output=csv";
        string fileCSV = Path.Combine(Application.persistentDataPath, "tempQuestions.csv");

        List<Question> questions = new List<Question>();

        try
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, fileCSV);
            }

            using (StreamReader reader = new StreamReader(fileCSV))
            {
                string currentLine;
                bool isHeader = true;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    string[] values = currentLine.Split(',');

                    Question question = new Question(
                        values[0], // Topic
                        values[1], // Question
                        values[2], // Answer1
                        values[3], // Answer2
                        values[4], // Answer3
                        values[5], // Correct Answer
                        values[6]  // Advice
                    );

                    questions.Add(question);
                }
            }
            return questions;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load questions: {e.Message}");
            return null;
        }
        finally
        {
            if (File.Exists(fileCSV))
            {
                File.Delete(fileCSV);
            }
        }
    }
}
