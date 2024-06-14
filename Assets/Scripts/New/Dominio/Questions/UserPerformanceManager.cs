using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPerformanceManager : MonoBehaviour
{
    public static UserPerformanceManager Instance;

    private Dictionary<string, FixedSizeQueue<char>> _userPerformance;

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

    public void InitializePerformance(List<string> allTopics)
    {
        _userPerformance = new Dictionary<string, FixedSizeQueue<char>>();

        UtilityFunctions.CopyDictionaryPerformance(DataStorage.LoadUserPerformance(allTopics), _userPerformance);

        foreach(var kvp in _userPerformance)
        {
            while(kvp.Value.Count() < 10)
            {
                kvp.Value.Enqueue('P');
            }
        }

    }

    public FixedSizeQueue<char> GetTopicPerformance(string topic)
    {
        if (_userPerformance.TryGetValue(topic, out FixedSizeQueue<char> specificTopicPerformance))
        {
            return specificTopicPerformance;
        }
        else
        {
            return new FixedSizeQueue<char>();
        }
    }

    public bool HasPendingAnswers(string topic)
    {
        FixedSizeQueue<char> topicPerformanceQueue = GetTopicPerformance(topic);
        return topicPerformanceQueue.Contains('P');
    }

    public void UpdatePerformance(List<Question> iterationQuestions)
    {
        foreach(var question in iterationQuestions)
        {
            FixedSizeQueue<char> specificTopicPerformance = GetTopicPerformance(question.topic);
            specificTopicPerformance.Enqueue(question.resultAnswer);
        }
    }

    void OnDestroy()
    {
        DataStorage.SaveUserPerformance(_userPerformance);
    }
}
