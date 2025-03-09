using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPerformanceManager : MonoBehaviour
{
    public static UserPerformanceManager Instance;

    private Dictionary<string, FixedSizeQueue<string>> _userPerformance;

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
        _userPerformance = new Dictionary<string, FixedSizeQueue<string>>();

        UtilityFunctions.CopyDictionaryPerformance(DataStorage.LoadUserPerformance(allTopics), _userPerformance);

        foreach(var kvp in _userPerformance)
        {
            while(kvp.Value.Count() < 10)
            {
                kvp.Value.Enqueue("P");
            }
        }

    }

    public FixedSizeQueue<string> GetTopicPerformance(string topic)
    {
        if (_userPerformance.TryGetValue(topic, out FixedSizeQueue<string> specificTopicPerformance))
        {
            return specificTopicPerformance;
        }
        else
        {
            return new FixedSizeQueue<string>();
        }
    }

    public bool HasPendingAnswers(string topic)
    {
        FixedSizeQueue<string> topicPerformanceQueue = GetTopicPerformance(topic);
        return topicPerformanceQueue.Contains("P");
    }

    public void UpdatePerformance(List<Question> iterationQuestions)
    {
        foreach(var question in iterationQuestions)
        {
            FixedSizeQueue<string> specificTopicPerformance = GetTopicPerformance(question.topic);
            specificTopicPerformance.Enqueue(question.resultAnswer);
        }
    }

    void OnDestroy()
    {
        DataStorage.SaveUserPerformance(_userPerformance);
    }
}
