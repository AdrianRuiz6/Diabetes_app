using Master.Auxiliar;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using Master.Domain.Questions;

namespace Master.Persistence.Questions
{
    public class DataStorage_Questions : IQuestionRepository
    {
        public void SaveCurrentQuestionIndex(int currentQuestionIndex)
        {
            PlayerPrefs.SetInt("CurrentQuestionIndex", currentQuestionIndex);
            PlayerPrefs.Save();
        }

        public int LoadCurrentQuestionIndex()
        {
            return PlayerPrefs.GetInt("CurrentQuestionIndex", 0);
        }

        public void SaveTimeLeftQuestionTimer(float timeLeft)
        {
            PlayerPrefs.SetFloat("LeftTimeQuestionTimer", timeLeft);
            PlayerPrefs.Save();
        }

        public float LoadTimeLeftQuestionTimer()
        {
            return PlayerPrefs.GetFloat("LeftTimeQuestionTimer", 0);
        }

        public void SaveUserPerformance(Dictionary<string, FixedSizeQueue<string>> userPerformance)
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


        public Dictionary<string, FixedSizeQueue<string>> LoadUserPerformance(List<string> allTopics)
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
                if (userPerformance.ContainsKey(topic) == false)
                {
                    userPerformance.Add(topic, new FixedSizeQueue<string>());
                }
            }

            return userPerformance;
        }

        public void ResetUserPerformance()
        {
            string path = $"{Application.persistentDataPath}/UserPerformanceData.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void SaveIterationQuestions(List<Question> iterationQuestions)
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

        public List<Question> LoadIterationQuestions()
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

        public void ResetIterationQuestions()
        {
            string path = $"{Application.persistentDataPath}/IterationQuestions.txt";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public List<Question> LoadQuestions()
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

        public int SaveURLQuestions(string url)
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

        public string LoadURLQuestions()
        {
            string defaultURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTmL6z_aRoGzC_IM7f85ga5WoVNv99d4oUYIsjTrLzUgKLUuzc4xXIY8n_TakI-OQ/pub?output=tsv";

            return PlayerPrefs.GetString("urlQuestions", defaultURL);
        }

        public void ResetQuestionURL()
        {
            PlayerPrefs.DeleteKey("urlQuestions");
        }
    }
}