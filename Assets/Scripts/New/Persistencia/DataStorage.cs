using Master.Domain.Economy;
using Master.Domain.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public static class DataStorage
{
    public static void SaveDisconnectionDate()
    {
        PlayerPrefs.SetString("DisconnectionDate", DateTime.Now.ToString());
    }

    public static DateTime LoadDisconnectionDate()
    {
        return DateTime.Parse(PlayerPrefs.GetString("DisconnectionDate", DateTime.Now.ToString()));
    }

    public static void SaveTimeLeftQuestionTimer(float timeLeft)
    {
        PlayerPrefs.SetFloat("LeftTimeQuestionTimer", timeLeft);
    }

    public static float LoadTimeLeftQuestionTimer()
    {
        return PlayerPrefs.GetFloat("LeftTimeQuestionTimer", 0);
    }

    public static void SaveTimeLeftIntervalIA(float timePassed)
    {
        PlayerPrefs.SetFloat("TimeLeft", timePassed);
    }

    public static float LoadTimeLeftIntervalIA()
    {
        return PlayerPrefs.GetFloat("TimeLeft", 0);
    }

    public static void SaveLastTimeInsulinUsed(DateTime? lastTimeInsulinUsed)
    {
        PlayerPrefs.SetString("LastTimeInsulinUsed", lastTimeInsulinUsed.ToString());
    }

    public static DateTime? LoadLastTimeInsulinUsed()
    {
        String timeSaved = PlayerPrefs.GetString("LastTimeInsulinUsed", string.Empty);
        if (timeSaved != string.Empty)
        {
            return DateTime.Parse(timeSaved);
        }

        return null;
    }

    public static void SaveLastTimeExerciseUsed(DateTime? lastTimeExerciseUsed)
    {
        PlayerPrefs.SetString("LastTimeExerciseUsed", lastTimeExerciseUsed.ToString());
    }

    public static DateTime? LoadLastTimeExerciseUsed()
    {
        String timeSaved = PlayerPrefs.GetString("LastTimeExerciseUsed", string.Empty);
        if (timeSaved != string.Empty)
        {
            return DateTime.Parse(timeSaved);
        }

        return null;
    }

    public static void SaveLastTimeFoodUsed(DateTime? lastTimeFoodUsed)
    {
        PlayerPrefs.SetString("LastTimeFoodUsed", lastTimeFoodUsed.ToString());
    }

    public static DateTime? LoadLastTimeFoodUsed()
    {
        String timeSaved = PlayerPrefs.GetString("LastTimeFoodUsed", string.Empty);
        if (timeSaved != string.Empty)
        {
            return DateTime.Parse(timeSaved);
        }

        return null;
    }

    public static void SaveGlycemia(float glycemiaValue)
    {
        PlayerPrefs.SetFloat("Glycemia", glycemiaValue);
    }

    public static float LoadGlycemia()
    {
        // return PlayerPrefs.GetFloat("Glycemia", 120); TODO: ponerlo en json
        return 120;
    }

    public static void SaveActivity(float activityValue)
    {
        PlayerPrefs.SetFloat("Activity", activityValue);
    }

    public static float LoadActivity()
    {
        //return PlayerPrefs.GetFloat("Activity", 50); TODO: ponerlo en json
        return 50;
    }

    public static void SaveHunger(float hungerValue)
    {
        PlayerPrefs.SetFloat("Hunger", hungerValue);
    }

    public static float LoadHunger()
    {
        //return PlayerPrefs.GetFloat("Hunger", 50); TODO: ponerlo en json
        return 50;
    }

    public static void SaveCurrentScore(int currentScore)
    {
        PlayerPrefs.SetInt("CurrentScore", currentScore);
    }

    public static int LoadCurrentScore()
    {
        return PlayerPrefs.GetInt("CurrentScore", 0);
    }

    public static void SaveHigherScore(int currentScore)
    {
        PlayerPrefs.SetInt("HigherScore", currentScore);
    }

    public static int LoadHigherScore()
    {
        return PlayerPrefs.GetInt("HigherScore", 0);
    }

    public static void SaveCoins(int coins)
    {
        PlayerPrefs.SetInt("Coins", coins);
    }

    public static int LoadCoins()
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