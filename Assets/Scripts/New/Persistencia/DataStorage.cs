using DG.Tweening.Plugins.Core.PathCore;
using Master.Domain.Economy;
using Master.Domain.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public static class DataStorage
{
    #region Volume
    public static float LoadMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    public static void SaveSoundEffectsVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        PlayerPrefs.Save();
    }

    public static float LoadSoundEffectsVolume()
    {
        return PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
    }
    #endregion

    #region Score
    public static void SaveCurrentScore(int currentScore)
    {
        PlayerPrefs.SetInt("CurrentScore", currentScore);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentScore()
    {
        return PlayerPrefs.GetInt("CurrentScore", 0);
    }

    public static void SaveHighestScore(int currentScore)
    {
        PlayerPrefs.SetInt("HigherScore", currentScore);
        PlayerPrefs.Save();
    }

    public static int LoadHighestScore()
    {
        return PlayerPrefs.GetInt("HigherScore", 0);
    }

    public static void SaveScoreInfo(List<ScoreRecordData> infoList)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "ScoreRecordData.txt");

        ScoreRecordDataList allScoreElements = new ScoreRecordDataList();
        foreach (ScoreRecordData scoreData in infoList)
        {
            allScoreElements.score.Add(scoreData);
        }

        string json = JsonUtility.ToJson(allScoreElements, true);

        using (StreamWriter streamWriter = new StreamWriter(path, false))
        {
            streamWriter.Write(json);
        }
    }

    public static List<ScoreRecordData> LoadScoreInfo()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "ScoreRecordData.txt");

        if (!File.Exists(path))
            return new List<ScoreRecordData>();

        string existingJson = null;

        using (StreamReader streamReader = new StreamReader(path))
        {
            existingJson = streamReader.ReadToEnd();
        }

        ScoreRecordDataList allScoreElements = JsonUtility.FromJson<ScoreRecordDataList>(existingJson);

        return allScoreElements.score;
    }
    #endregion

    #region Graph

    #region Initial and Finish time
    public static void SaveInitialTime(TimeSpan initialTime)
    {
        int hour = initialTime.Hours;
        PlayerPrefs.SetInt("InitialTime", hour);
        PlayerPrefs.Save();
    }

    public static TimeSpan LoadInitialTime()
    {
        int hour = PlayerPrefs.GetInt("InitialTime", 14);
        return new TimeSpan(hour, 0, 0);
    }

    public static void SaveFinishTime(TimeSpan finishTime)
    {
        int hour = finishTime.Hours + 1;
        PlayerPrefs.SetInt("FinishTime", hour);
        PlayerPrefs.Save();
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

    public static void ResetInsulinGraph()
    {
        string path = $"{Application.persistentDataPath}/InsulinGraphData.txt";
        ResetActionsGraph(path);
    }

    public static void ResetFoodGraph()
    {
        string path = $"{Application.persistentDataPath}/FoodGraphData.txt";
        ResetActionsGraph(path);
    }

    public static void ResetExerciseGraph()
    {
        string path = $"{Application.persistentDataPath}/ExerciseGraphData.txt";
        ResetActionsGraph(path);
    }

    private static void ResetActionsGraph(string path)
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
            DateTime currentDate = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            if (currentDate.Date != DateTime.Now.Date)
            {
                newButtonList.buttonList.Add(buttonData);
            }
        }

        string json = JsonUtility.ToJson(newButtonList, true);
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(json);
        }
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
            if (requestedDate.Value.Date == currentDate.Date && LimitHours.Instance.IsInRange(currentDate.TimeOfDay))
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
    public static void ResetGlycemiaGraph()
    {
        string path = $"{Application.persistentDataPath}/GlycemiaGraphData.txt";
        ResetAttributeGraph(path);
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
    public static void ResetHungerGraph()
    {
        string path = $"{Application.persistentDataPath}/HungerGraphData.txt";
        ResetAttributeGraph(path);
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
    public static void ResetActivityGraph()
    {
        string path = $"{Application.persistentDataPath}/ActivityGraphData.txt";
        ResetAttributeGraph(path);
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

    private static void ResetAttributeGraph(string path)
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
            DateTime currentDate = DateTime.Parse(attributeData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
            if (currentDate.Date != DateTime.Now.Date)
            {
                newAttributeList.attributeList.Add(attributeData);
            }
        }

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
            if (requestedDate.Value.Date == currentDate.Date && LimitHours.Instance.IsInRange(currentDate.TimeOfDay))
            {
                askedDateAttributeDictionary.Add(currentDate, attributeData.Value);
            }
        }

        return askedDateAttributeDictionary;
    }
    #endregion
    #endregion

    #region Economy
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
    #endregion

    #region Conection
    public static void SaveIsFirstUsage()
    {
        PlayerPrefs.SetInt("IsFirstUsage", 1);
        PlayerPrefs.Save();
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
        PlayerPrefs.Save();
    }

    public static DateTime LoadDisconnectionDate()
    {
        return DateTime.Parse(PlayerPrefs.GetString("DisconnectionDate", DateTime.Now.ToString()));
    }

    public static void SaveLastIterationStartTime(DateTime startTimeInterval)
    {
        PlayerPrefs.SetString("LastIntervalStartTime", startTimeInterval.ToString());
        PlayerPrefs.Save();
    }

    public static DateTime LoadLastIterationStartTime()
    {
        return DateTime.Parse(PlayerPrefs.GetString("LastIntervalStartTime", DateTime.Now.Date.ToString()));
    }
    #endregion

    #region Attributes
    public static void SaveLastTimeInsulinUsed(DateTime? lastTimeInsulinUsed)
    {
        PlayerPrefs.SetString("LastTimeInsulinUsed", lastTimeInsulinUsed.ToString());
        PlayerPrefs.Save();
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
        PlayerPrefs.Save();
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
        PlayerPrefs.Save();
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

    public static void SaveGlycemia(int glycemiaValue)
    {
        PlayerPrefs.SetInt("Glycemia", glycemiaValue);
        PlayerPrefs.Save();
    }

    public static int LoadGlycemia()
    {
        return PlayerPrefs.GetInt("Glycemia", AttributeManager.Instance.initialGlycemiaValue);
    }

    public static void SaveActivity(int activityValue)
    {
        PlayerPrefs.SetInt("Activity", activityValue);
        PlayerPrefs.Save();
    }

    public static int LoadActivity()
    {
        return PlayerPrefs.GetInt("Activity", AttributeManager.Instance.initialActivityValue);
    }

    public static void SaveHunger(int hungerValue)
    {
        PlayerPrefs.SetInt("Hunger", hungerValue);
        PlayerPrefs.Save();
    }

    public static int LoadHunger()
    {
        return PlayerPrefs.GetInt("Hunger", AttributeManager.Instance.initialHungerValue);
    }
    #endregion

    #region Questions
    public static void SaveCurrentQuestionIndex(int currentQuestionIndex)
    {
        PlayerPrefs.SetInt("CurrentQuestionIndex", currentQuestionIndex);
        PlayerPrefs.Save();
    }

    public static int LoadCurrentQuestionIndex()
    {
        return PlayerPrefs.GetInt("CurrentQuestionIndex", 0);
    }

    public static void ResetCurrentQuestionIndex()
    {
        PlayerPrefs.SetInt("CurrentQuestionIndex", 0);
        PlayerPrefs.Save();
    }

    public static void SaveTimeLeftQuestionTimer(float timeLeft)
    {
        PlayerPrefs.SetFloat("LeftTimeQuestionTimer", timeLeft);
        PlayerPrefs.Save();
    }

    public static float LoadTimeLeftQuestionTimer()
    {
        return PlayerPrefs.GetFloat("LeftTimeQuestionTimer", 0);
    }

    public static void SaveUserPerformance(Dictionary<string, FixedSizeQueue<string>> userPerformance)
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


    public static Dictionary<string, FixedSizeQueue<string>> LoadUserPerformance(List<string> allTopics)
    {
        string path = $"{Application.persistentDataPath}/UserPerformanceData.txt";
        Dictionary<string, FixedSizeQueue<string>> userPerformance = new Dictionary<string, FixedSizeQueue<string>>();

        if (!File.Exists(path))
        {
            foreach (string topic in allTopics)
            {
                userPerformance.Add(topic, new FixedSizeQueue<string>());
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
            FixedSizeQueue<string> queue = new FixedSizeQueue<string>(data.performanceData);

            userPerformance.Add(data.topic, queue);
        }

        foreach (string topic in allTopics)
        {
            if(userPerformance.ContainsKey(topic) == false)
            {
                userPerformance.Add(topic, new FixedSizeQueue<string>());
            }
        }

        return userPerformance;
    }

    public static void ResetUserPerformance()
    {
        string path = $"{Application.persistentDataPath}/UserPerformanceData.txt";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void SaveIterationQuestions(List<Question> iterationQuestions)
    {
        string path = $"{Application.persistentDataPath}/IterationQuestions.txt";

        IterationQuestionsDataList newIterationQuestions = new IterationQuestionsDataList();
        foreach (Question question in iterationQuestions)
        {
            newIterationQuestions.questions.Add(question);
        }

        string json = JsonUtility.ToJson(newIterationQuestions, true);

        using (StreamWriter streamWriter = new StreamWriter(path, false))
        {
            streamWriter.Write(json);
        }
    }

    public static List<Question> LoadIterationQuestions()
    {
        string path = $"{Application.persistentDataPath}/IterationQuestions.txt";
        if (!File.Exists(path))
            return new List<Question>();

        string existingJson = null;

        using (StreamReader streamReader = new StreamReader(path))
        {
            existingJson = streamReader.ReadToEnd();
        }

        IterationQuestionsDataList currentIterationQuestions = JsonUtility.FromJson<IterationQuestionsDataList>(existingJson);

        return currentIterationQuestions.questions;
    }

    public static void ResetIterationQuestions()
    {
        string path = $"{Application.persistentDataPath}/IterationQuestions.txt";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static List<Question> LoadQuestions()
    {
        string url = LoadURLQuestions();
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

                    string[] values = currentLine.Split('\t');

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
            Debug.LogError($"Error inesperado al cargar preguntas: {e.Message}");
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

    public static int SaveURLQuestions(string url)
    {
        // Comprobar errores
        string fileCSV = System.IO.Path.Combine(Application.persistentDataPath, "tempQuestions.csv");

        try
        {
            if (!url.Contains("output=tsv"))
            {
                // -5 significa que la URL no tiene formato tsv
                return -5;
            }

            // Validar que el URL no está vacío
            if (string.IsNullOrWhiteSpace(url))
            {
                // -1 significa que la URL está vacía
                Debug.LogError("El URL proporcionado está vacío.");
                return -1;
            }

            // Intentar descargar el archivo CSV desde la URL
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(url, fileCSV);
                }
                catch (WebException webEx)
                {
                    // -2 significa que no carga el archivo
                    Debug.LogError($"Error descargando archivo desde la URL: {webEx.Message}");
                    return -2;
                }
            }

            // Leer y procesar el archivo
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

                    string[] values = currentLine.Split('\t');

                    if (values.Length != 7)
                    {
                        // -3 significa que el número de columnas del archivo no es válido
                        Debug.LogError($"Formato inválido en línea: {values.Length}");
                        return -3;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            // -4 significa que ha habido un error inesperado
            Debug.LogError($"Error inesperado al cargar preguntas: {e.Message}");
            return -4;
        }
        finally
        {
            if (File.Exists(fileCSV))
            {
                File.Delete(fileCSV);
            }
        }

        // Guardar URL de la pregunta: devolver 0 significa éxito.
        PlayerPrefs.SetString("urlQuestions", url);

        return 0;
    }

    public static string LoadURLQuestions()
    {
        string defaultURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTmL6z_aRoGzC_IM7f85ga5WoVNv99d4oUYIsjTrLzUgKLUuzc4xXIY8n_TakI-OQ/pub?output=tsv";

        return PlayerPrefs.GetString("urlQuestions", defaultURL);
    }

    public static void ResetQuestionURL()
    {
        PlayerPrefs.DeleteKey("urlQuestions");
    }
    #endregion
}