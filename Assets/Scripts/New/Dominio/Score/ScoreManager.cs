using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int _currentScore;
    [SerializeField] private int _highestScore;

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

    private void OnDestroy()
    {
        DataStorage.SaveCurrentScore(_currentScore);
        DataStorage.SaveHighestScore(_highestScore);
    }

    private void Start()
    {
        _highestScore = DataStorage.LoadHighestScore();
        _currentScore = DataStorage.LoadCurrentScore();

        if (DataStorage.LoadDisconnectionDate().Date < DateTime.Now.Date)
        {
            CheckHighestScore();
            _currentScore = 0;
        }

        StartCoroutine(CheckScoreAtMidnight());
    }

    private void CheckHighestScore()
    {
        if(_currentScore > _highestScore)
        {
            _highestScore = _currentScore;
            GameEventsScore.OnModifyHighestScore?.Invoke(_highestScore);
        }
    }

    private IEnumerator CheckScoreAtMidnight()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.AddDays(1).Date;

            while (DateTime.Now < nextMidnight)
            {
                yield return new WaitForSeconds(1);
            }

            CheckHighestScore();
            _currentScore = 0;
            GameEventsScore.OnMidnight?.Invoke();
            break;
        }
    }

    public void AddScore(int score, DateTime? time, string activity)
    {
        _currentScore += score;
        GameEventsScore.OnModifyCurrentScore?.Invoke(score, time, activity);
    }

    public void SubstractScore(int score, DateTime? time, string activity)
    {
        if(_currentScore - score < 0)
        {
            _currentScore = 0;
            GameEventsScore.OnModifyCurrentScore?.Invoke(-score, time, activity);
        }
        else
        {
            _currentScore -= score;
            GameEventsScore.OnModifyCurrentScore?.Invoke(-score, time, activity);
        }
    }
}
