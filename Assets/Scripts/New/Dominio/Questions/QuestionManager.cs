using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.Android;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    public Dictionary<string, List<Question>> _allQuestions { get; private set; }
    private List<Question> _iterationQuestions;
    public int currentQuestionIndex { get; private set; } = 0;
    public int maxQuestionIndex { get; private set; } = 10;

    System.Random random = new System.Random();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        GameEvents_Questions.OnConfirmChangeQuestions += InitializeQuestions;
    }

    private void OnDestroy()
    {
        DataStorage.SaveCurrentQuestionIndex(currentQuestionIndex);
        DataStorage.SaveIterationQuestions(_iterationQuestions);

        GameEvents_Questions.OnConfirmChangeQuestions -= InitializeQuestions;
    }

    void Start()
    {
        InitializeQuestions();
    }

    public void InitializeQuestions()
    {
        _allQuestions = new Dictionary<string, List<Question>>();
        List<string> allTopics = new List<string>();
        List<Question> allQuestionsList = new List<Question>();
        UtilityFunctions.CopyList(DataStorage.LoadQuestions(), allQuestionsList);

        if (allQuestionsList == null || allQuestionsList.Count == 0)
        {
            StartCoroutine(TimerRetry());
            return;
        }

        foreach (Question question in allQuestionsList)
        {
            if (_allQuestions.ContainsKey(question.topic))
            {
                _allQuestions[question.topic].Add(question);
            }
            else
            {
                List<Question> questions = new List<Question>() { question };
                _allQuestions.Add(question.topic, questions);
            }
        }

        foreach (var kvp in _allQuestions)
        {
            allTopics.Add(kvp.Key);
        }

        UserPerformanceManager.Instance.InitializePerformance(allTopics);

        _iterationQuestions = DataStorage.LoadIterationQuestions();
        if(DataStorage.LoadCurrentQuestionIndex() - 1 < 0)
        {
            currentQuestionIndex = 0;
        }
        else
        {
            currentQuestionIndex = DataStorage.LoadCurrentQuestionIndex() - 1;
        }
        
        if(_iterationQuestions.Count == 0)
        {
            GameEvents_Questions.OnExecuteQuestionSearch?.Invoke();
        }
        else
        {
            GameEvents_Questions.OnFinalizedCreationQuestions?.Invoke();
        }
    }

    IEnumerator TimerRetry()
    {
        yield return new WaitForSeconds(5);
        InitializeQuestions();
    }

    #region Calculate Proportions
    public Dictionary<string, float> CalculateAppearanceProportions()
    {
        // Calculates the percentage of successes in _iterationQuestions.
        Dictionary<string, float> topicAccuracy = CalculateAccuracy();

        // Calculates inverse percentage based on topicAccuracy. 
        Dictionary<string, float> inverseTopicAccuracy = CalculatePriority(topicAccuracy);

        // Normalizes inverse percentages.
        Dictionary<string, float> normalizedProportions = CalculateNormal(inverseTopicAccuracy);

        return normalizedProportions;
    }

    private Dictionary<string, float> CalculateAccuracy()
    {
        Dictionary<string, float> topicAccuracy = new Dictionary<string, float>();

        foreach (var topic in _allQuestions.Keys)
        {
            FixedSizeQueue<string> userPerformanceTopic = UserPerformanceManager.Instance.GetTopicPerformance(topic);

            if(userPerformanceTopic.Count() != 0)
            {
                int correctAnswers = 0;
                foreach (string answer in userPerformanceTopic)
                {
                    if (answer == "S")
                        correctAnswers++;
                }
                float accuracy = (float)correctAnswers / maxQuestionIndex * 100;
                topicAccuracy[topic] = accuracy;
                Debug.Log($"Accuracy for {topic}: {accuracy}");
            }
        }
        return topicAccuracy;
    }

    private Dictionary<string, float> CalculatePriority(Dictionary<string, float> topicAccuracy)
    {
        Dictionary<string, float> topicPriority = new Dictionary<string, float>();
        foreach (var topic in topicAccuracy.Keys)
        {
            float priority = 100 - topicAccuracy[topic];
            topicPriority[topic] = priority;
            Debug.Log($"Priority for {topic}: {priority}");
        }

        return topicPriority;
    }

    private Dictionary<string, float> CalculateNormal(Dictionary<string, float> topicPriority)
    {
        float sumPriority = topicPriority.Values.Sum();
        Dictionary<string, float> proportions = new Dictionary<string, float>();
        foreach (var topic in topicPriority.Keys)
        {
            float proportion = (topicPriority[topic] / sumPriority) * 10;
            proportions[topic] = proportion;
            Debug.Log($"Proportion for {topic}: {proportion}");
        }

        return proportions;
    }
    #endregion

    public Dictionary<string, int> AdjustQuestionCount(Dictionary<string, float> appearanceProportions)
    {
        Dictionary<string, int> amountQuestionsPerTopic = new Dictionary<string, int>();

        // Calculates the amount of questions per topic.
        foreach (var topic in appearanceProportions.Keys)
        {
            amountQuestionsPerTopic[topic] = Mathf.RoundToInt(appearanceProportions[topic]);
            Debug.Log($"Amount of questions for {topic}: {amountQuestionsPerTopic[topic]}"); // Debug Log
        }

        int totalAmountQuestions = amountQuestionsPerTopic.Values.Sum();

        while (totalAmountQuestions != 10)
        {
            if (totalAmountQuestions > 10)
            {
                List<string> descendingTopicAmount = amountQuestionsPerTopic.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                foreach (var topic in descendingTopicAmount)
                {
                    amountQuestionsPerTopic[topic]--;
                    totalAmountQuestions--;
                    Debug.Log($"Decreased questions for {topic}: {amountQuestionsPerTopic[topic]}"); // Debug Log

                    if (totalAmountQuestions == 10)
                        break;
                }
            }
            else if (totalAmountQuestions < 10)
            {
                List<string> ascendingTopicAmount = amountQuestionsPerTopic.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                foreach (var topic in ascendingTopicAmount)
                {
                    amountQuestionsPerTopic[topic]++;
                    totalAmountQuestions++;
                    Debug.Log($"Increased questions for {topic}: {amountQuestionsPerTopic[topic]}"); // Debug Log

                    if (totalAmountQuestions == 10)
                        break;
                }
            }
        }

        return amountQuestionsPerTopic;
    }

    public void SelectRandomQuestions(Dictionary<string, int> amountQuestionsPerTopic = null)
    {
        if (amountQuestionsPerTopic == null)
        {
            amountQuestionsPerTopic = new Dictionary<string, int>();

            while (amountQuestionsPerTopic.Values.Sum() < 10)
            {
                foreach (string topic in _allQuestions.Keys)
                {
                    if (UserPerformanceManager.Instance.HasPendingAnswers(topic))
                        if (amountQuestionsPerTopic.ContainsKey(topic))
                        {
                            amountQuestionsPerTopic[topic]++;
                        }
                        else
                        {
                            amountQuestionsPerTopic.Add(topic, 1);
                        }

                    if (amountQuestionsPerTopic.Values.Sum() == 10)
                        break;
                }
            }
        }

        foreach (string topic in amountQuestionsPerTopic.Keys)
        {
            int amount = amountQuestionsPerTopic[topic];

            while (amount > 0)
            {
                AddRandomQuestion(topic, amountQuestionsPerTopic);
                amount--;
            }
        }
    }

    private void AddRandomQuestion(string topic, Dictionary<string, int> amountQuestionsPerTopic)
    {
        bool repeatedQuestion = false;
        Question newQuestion;
        int attempts = 0;
        int maxAttempts = 100;

        do
        {
            newQuestion = _allQuestions[topic][random.Next(_allQuestions[topic].Count)];
            repeatedQuestion = _iterationQuestions.Contains(newQuestion);
            attempts++;
        } while (repeatedQuestion && attempts < maxAttempts);

        _iterationQuestions.Add(newQuestion);
    }

    public Question NextQuestion()
    {
        if (currentQuestionIndex < maxQuestionIndex)
        {
            Question nextQuestion = _iterationQuestions[currentQuestionIndex];
            currentQuestionIndex += 1;
            return nextQuestion;
        }
        else
        {
            SearchNewQuestions();
            return null;
        }
    }

    private void SearchNewQuestions()
    {
        UserPerformanceManager.Instance.UpdatePerformance(_iterationQuestions);
        _iterationQuestions = new List<Question>();
        currentQuestionIndex = 0;

        GameEvents_Questions.OnExecuteQuestionSearch?.Invoke();
    }

    public void RandomizeOrderQuestions()
    {
        UtilityFunctions.RandomizeList(_iterationQuestions);

        foreach (Question question in _iterationQuestions)
        {
            question.RandomizeOrderAnswer();
        }
    }

    public string GetCorrectAnswer()
    {
        if(currentQuestionIndex - 1 == 0)
        {
            return _iterationQuestions[0].correctAnswer;
        }
        return _iterationQuestions[currentQuestionIndex - 1].correctAnswer;
    }
}
