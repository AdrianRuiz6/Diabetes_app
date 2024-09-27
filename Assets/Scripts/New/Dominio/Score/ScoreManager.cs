using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int _currentScore;
    [SerializeField] private int _highestScore;
    private DateTime _lastTimeDisconnection;

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
        _currentScore = DataStorage.LoadCurrentScore();
        GameEventsScore.OnModifyCurrentScore?.Invoke(_currentScore);
        _highestScore = DataStorage.LoadHighestScore();
        GameEventsScore.OnModifyHighestScore?.Invoke(_highestScore);
        _lastTimeDisconnection = DataStorage.LoadDisconnectionDate();

        CheckPastMidnights();

        StartCoroutine(CheckScoreAtMidnight());
    }

    private void CheckHighestScore()
    {
        if(_currentScore > _highestScore)
        {
            _highestScore = _currentScore;
            GameEventsScore.OnModifyHighestScore?.Invoke(_highestScore);
        }

        _currentScore = 0;
        GameEventsScore.OnModifyCurrentScore?.Invoke(_currentScore);
    }

    private void CheckPastMidnights()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeDifference = currentTime.Date - _lastTimeDisconnection.Date;

        if(Mathf.Abs(timeDifference.Days) >= 1)
        {
            CheckHighestScore();
        }
    }

    private IEnumerator CheckScoreAtMidnight()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.AddDays(1).Date;

            yield return new WaitForSeconds((float)(nextMidnight - now).TotalSeconds);

            CheckHighestScore();
        }
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        GameEventsScore.OnModifyCurrentScore?.Invoke(_currentScore);
    }

    public void SubstractScore(int score)
    {
        if(_currentScore - score < 0)
        {
            _currentScore = 0;
            GameEventsScore.OnModifyCurrentScore?.Invoke(0);
        }
        else
        {
            _currentScore -= score;
            GameEventsScore.OnModifyCurrentScore?.Invoke(_currentScore);
        }
    }
}
