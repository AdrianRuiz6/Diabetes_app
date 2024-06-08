using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPerformanceManager : MonoBehaviour
{
    public static UserPerformanceManager Instance;

    private Dictionary<string, FixedSizeQueue<char>> userPerformance;

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
        UtilityFunctions.CopyDictionaryPerformance(DataStorage.LoadUserPerformance(), userPerformance);

        foreach(var kvp in userPerformance)
        {
            // TODO
        }
    }

    private FixedSizeQueue<char> GetTopicPerformance(string topic)
    {
        if (userPerformance.TryGetValue(topic, out FixedSizeQueue<char> performance))
        {
            return performance;
        }
        else
        {
            return null;
        }
    }

    private void UpdatePerformance(List<Question> iterationQuestions)
    {
        foreach(var question in iterationQuestions)
        {
            FixedSizeQueue<char> performance = GetTopicPerformance(question.topic);
            // TODO
        }
    }
}
