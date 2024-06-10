using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPerformanceManager : MonoBehaviour
{
    public static UserPerformanceManager Instance;

    private Dictionary<string, FixedSizeQueue<char>> _userTopicPerformance;

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

        InitializePerformance();
    }

    private void InitializePerformance()
    {
        UtilityFunctions.CopyDictionaryPerformance(DataStorage.LoadUserPerformance(), _userTopicPerformance);

        foreach(var kvp in _userTopicPerformance)
        {
            while(kvp.Value.Count() < 10)
            {
                kvp.Value.Enqueue('P');
            }
        }
    }

    public FixedSizeQueue<char> GetTopicPerformance(string topic)
    {
        if (_userTopicPerformance.TryGetValue(topic, out FixedSizeQueue<char> specificTopicPerformance))
        {
            return specificTopicPerformance;
        }
        else
        {
            return null;
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
}
