using DG.Tweening.Plugins.Core.PathCore;
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
using UnityEngine.Assertions.Must;

public static class DataStorage
{
    #region Graph
    #region Initial and Finish time
    public static void SaveInitialTime(TimeSpan initialTime)
    {
        int hour = initialTime.Hours;
        PlayerPrefs.SetInt("InitialTime", hour);
    }

    public static TimeSpan LoadInitialTime()
    {
        int hour = PlayerPrefs.GetInt("InitialTime", 20);
        return new TimeSpan(hour, 56, 0);
    }

    public static void SaveFinishTime(TimeSpan finishTime)
    {
        int hour = finishTime.Hours + 1;
        PlayerPrefs.SetInt("FinishTime", hour);
    }

    public static TimeSpan LoadFinishTime()
    {
        int hour = PlayerPrefs.GetInt("FinishTime", 21);
        return new TimeSpan(hour - 1, 59, 0);
    }
    #endregion

    #region ButtonGraph
    public static void SaveInsulinGraph(DateTime? dateTime, string information)
    {
        string path = $"{Application.persistentDataPath}/InsulinGraphData.txt";
        SaveButtonGraph(dateTime, information, path);
    }

    public static Dictionary<DateTime, string> LoadInsulinGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/InsulinGraphData.txt";
        return LoadButtonGraph(path, requestedDate);
    }
    public static void SaveFoodGraph(DateTime? dateTime, string information)
    {
        string path = $"{Application.persistentDataPath}/FoodGraphData.txt";
        SaveButtonGraph(dateTime, information, path);
    }

    public static Dictionary<DateTime, string> LoadFoodGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/FoodGraphData.txt";
        return LoadButtonGraph(path, requestedDate);
    }

    public static void SaveExerciseGraph(DateTime? dateTime, string information)
    {
        string path = $"{Application.persistentDataPath}/ExerciseGraphData.txt";
        SaveButtonGraph(dateTime, information, path);
    }

    public static Dictionary<DateTime, string> LoadExerciseGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/ExerciseGraphData.txt";
        return LoadButtonGraph(path, requestedDate);
    }

    private static void SaveButtonGraph(DateTime? dateTime, string information, string path)
    {
        ButtonDataList originalButtonList = new ButtonDataList();
        ButtonDataList newButtonList = new ButtonDataList();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            originalButtonList = JsonUtility.FromJson<ButtonDataList>(existingJson) ?? new ButtonDataList();
        }

        foreach (ButtonData buttonData in originalButtonList.buttonList)
        {
            // DateTime currentDateAndTime = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            newButtonList.buttonList.Add(buttonData);
        }

        ButtonData newButtonData = new ButtonData(dateTime, information);
        newButtonList.buttonList.Add(newButtonData);

        string json = JsonUtility.ToJson(newButtonList, true);
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }

    private static Dictionary<DateTime, string> LoadButtonGraph(string path, DateTime? requestedDate)
    {
        if (!File.Exists(path))
        {
            return new Dictionary<DateTime, string>();
        }

        string existingJson = null;
        using (StreamReader streamReader = new StreamReader(path))
        {
            existingJson = streamReader.ReadToEnd();
        }

        ButtonDataList buttonDataList = JsonUtility.FromJson<ButtonDataList>(existingJson);

        Dictionary<DateTime, string> askedDateButtonDictionary = new Dictionary<DateTime, string>();
        foreach (ButtonData buttonData in buttonDataList.buttonList)
        {
            DateTime currentDate = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            if (requestedDate.Value.Date == currentDate.Date)
            {
                askedDateButtonDictionary.Add(currentDate, buttonData.Information);
            }
        }

        return askedDateButtonDictionary;
    }
    #endregion

    #region AttributeGraph
    public static void SaveGlycemiaGraph(DateTime? dateTime, int number)
    {
        string path = $"{Application.persistentDataPath}/GlycemiaGraphData.txt";
        SaveAttributeGraph(dateTime, number, path);
    }
    public static Dictionary<DateTime, int> LoadGlycemiaGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/GlycemiaGraphData.txt";
        return LoadAttributeGraph(path, requestedDate);
    }

    public static void SaveHungerGraph(DateTime? dateTime, int number)
    {
        string path = $"{Application.persistentDataPath}/HungerGraphData.txt";
        SaveAttributeGraph(dateTime, number, path);
    }
    public static Dictionary<DateTime, int> LoadHungerGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/HungerGraphData.txt";
        return LoadAttributeGraph(path, requestedDate);
    }

    public static void SaveActivityGraph(DateTime? dateTime, int number)
    {
        string path = $"{Application.persistentDataPath}/ActivityGraphData.txt";
        SaveAttributeGraph(dateTime, number, path);
    }
    public static Dictionary<DateTime, int> LoadActivityGraph(DateTime? requestedDate)
    {
        string path = $"{Application.persistentDataPath}/ActivityGraphData.txt";
        return LoadAttributeGraph(path, requestedDate);
    }

    private static void SaveAttributeGraph(DateTime? dateTime, int number, string path)
    {
        AttributeDataList originalAttributeList = new AttributeDataList();
        AttributeDataList newAttributeList = new AttributeDataList();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            originalAttributeList = JsonUtility.FromJson<AttributeDataList>(existingJson) ?? new AttributeDataList();
        }

        foreach (AttributeData attributeData in originalAttributeList.attributeList)
        {
            // DateTime currentDateAndTime = DateTime.Parse(attributeData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            newAttributeList.attributeList.Add(attributeData);
        }

        AttributeData newAttributeData = new AttributeData(dateTime, number);
        newAttributeList.attributeList.Add(newAttributeData);

        string json = JsonUtility.ToJson(newAttributeList, true);
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
    }
    private static Dictionary<DateTime, int> LoadAttributeGraph(string path, DateTime? requestedDate)
    {
        if (!File.Exists(path))
        {
            return new Dictionary<DateTime, int>();
        }

        string existingJson = null;
        using(StreamReader streamReader = new StreamReader(path))
        {
            existingJson = streamReader.ReadToEnd();
        }

        AttributeDataList attributeDataList = JsonUtility.FromJson<AttributeDataList>(existingJson);

        Dictionary<DateTime, int> askedDateAttributeDictionary = new Dictionary<DateTime, int>();
        foreach (AttributeData attributeData in attributeDataList.attributeList)
        {
            DateTime currentDate = DateTime.Parse(attributeData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            if (requestedDate.Value.Date == currentDate.Date)
            {
                askedDateAttributeDictionary.Add(currentDate, attributeData.Value);
            }
        }

        return askedDateAttributeDictionary;
    }
    #endregion
    #endregion
    public static void SaveIsFirstUsage()
    {
        PlayerPrefs.SetInt("IsFirstUsage", 1);
    }

    public static bool LoadIsFirstUsage()
    {
        int firstUsage = PlayerPrefs.GetInt("IsFirstUsage", 0);

        if(firstUsage == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

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

    public static void SaveRestTimeIntervalSimulator(float timePassed)
    {
        PlayerPrefs.SetFloat("TimeLeft", timePassed);
    }

    public static float LoadRestTimeIntervalSimulator()
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

    public static int LoadGlycemia()
    {
        //return PlayerPrefs.GetInt("Glycemia", AttributeManager.Instance.initialGlycemiaValue);
        return 120;
    }

    public static void SaveActivity(float activityValue)
    {
        PlayerPrefs.SetFloat("Activity", activityValue);
    }

    public static int LoadActivity()
    {
        //return PlayerPrefs.GetInt("Activity", AttributeManager.Instance.initialActivityValue);
        return 50;
    }

    public static void SaveHunger(float hungerValue)
    {
        PlayerPrefs.SetFloat("Hunger", hungerValue);
    }

    public static int LoadHunger()
    {
        //return PlayerPrefs.GetInt("Hunger", AttributeManager.Instance.initialHungerValue);
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

    public static void SaveHighestScore(int currentScore)
    {
        PlayerPrefs.SetInt("HigherScore", currentScore);
    }

    public static int LoadHighestScore()
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

        string json = JsonUtility.ToJson(dataList, true);

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
        string fileCSV = System.IO.Path.Combine(Application.persistentDataPath, "tempQuestions.csv");

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