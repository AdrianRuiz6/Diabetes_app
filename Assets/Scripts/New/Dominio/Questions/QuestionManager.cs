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
    }

    void Start()
    {
        _allQuestions = null;
        _iterationQuestions = null;
        currentQuestionIndex = 0;

        InitializeQuestions();
    }

    public void InitializeQuestions()
    {
        List<Question> allQuestionsList = new List<Question>();
        UtilityFunctions.CopyList(DataStorage.LoadQuestions(), allQuestionsList);

        if (_allQuestions == null)
        {
            GameEventsQuestions.OnNotLoadedSheetQuestions?.Invoke(); //TODO EN EL OTRO LADO
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
    }

    #region Calculate Proportions
    public Dictionary<string, float> CalculateAppearanceProportions()
    {
        // Calculates the percentage of successes in _iterationQuestions.
        Dictionary<string, float> topicAccuracy = CalculateAccuracy();

        // Calculates inverse percentage based on topicAccuracy. 
        Dictionary<string, float> inverseTopicAccuracy = CalculateInverse(topicAccuracy);

        // Normalizes inverse percentages.
        Dictionary<string, float> normalizedProportions = CalculateNormal(inverseTopicAccuracy);

        return normalizedProportions;
    }

    private Dictionary<string, float> CalculateAccuracy()
    {
        Dictionary<string, float> topicAccuracy = new Dictionary<string, float>();

        foreach (var topic in _allQuestions.Keys)
        {
            List<Question> topicQuestions = _allQuestions[topic];
            int correctAnswers = 0;
            foreach (var question in topicQuestions)
            {
                if (question.isCorrect())
                    correctAnswers++;
            }
            float accuracy = (float)(correctAnswers / maxQuestionIndex) * 100;
            topicAccuracy[topic] = accuracy;
        }
        return topicAccuracy;
    }

    private Dictionary<string, float> CalculateInverse(Dictionary<string, float> topicAccuracy)
    {
        Dictionary<string, float> inverseAccuracy = new Dictionary<string, float>();
        foreach (var topic in topicAccuracy.Keys)
        {
            inverseAccuracy[topic] = 100 - topicAccuracy[topic];
        }

        return inverseAccuracy;
    }

    private Dictionary<string, float> CalculateNormal(Dictionary<string, float> inverseAccuracy)
    {
        float sumInverseAccuracy = inverseAccuracy.Values.Sum();
        Dictionary<string, float> proportions = new Dictionary<string, float>();
        foreach (var topic in inverseAccuracy.Keys)
        {
            proportions[topic] = (inverseAccuracy[topic] / sumInverseAccuracy) * 100;
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
            amountQuestionsPerTopic[topic] = (int)Mathf.Round(appearanceProportions[topic] / 10);
        }

        int totalAmountQuestions = amountQuestionsPerTopic.Values.Sum();

        while (totalAmountQuestions != 10)
        {
            if (totalAmountQuestions > 10)
            {
                List<string> descendingTopicAmount = appearanceProportions.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                foreach (var topic in descendingTopicAmount)
                {
                    amountQuestionsPerTopic[topic]--;
                    totalAmountQuestions--;

                    if (totalAmountQuestions == 10)
                        break;
                }
            }
            else if (totalAmountQuestions < 10)
            {
                List<string> descendingTopicAmount = appearanceProportions.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

                foreach (var topic in descendingTopicAmount)
                {
                    amountQuestionsPerTopic[topic]++;
                    totalAmountQuestions++;

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
                        amountQuestionsPerTopic[topic] += 1;

                    if (amountQuestionsPerTopic.Values.Sum() == 10)
                        break;
                }
            }
        }

        foreach (string topic in amountQuestionsPerTopic.Keys)
        {
            while (amountQuestionsPerTopic[topic] > 0)
            {
                AddRandomQuestion(topic, amountQuestionsPerTopic);
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
        amountQuestionsPerTopic[topic]--;
    }

    public Question NextQuestion()
    {
        currentQuestionIndex += 1;

        if (currentQuestionIndex < maxQuestionIndex)
        {
            return _iterationQuestions[currentQuestionIndex];
        }
        else
        {
            return null;
        }
    }

    public void ResetQuestions()
    {
        UserPerformanceManager.Instance.UpdatePerformance(_iterationQuestions);
        _iterationQuestions = null;
        currentQuestionIndex = 0;

        GameEventsQuestions.OnExecuteQuestionSearch?.Invoke();
    }
}
