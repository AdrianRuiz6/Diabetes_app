using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int _currentScore;
    [SerializeField] private int _higherScore;
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
        DataStorage.SaveHigherScore(_higherScore);
    }

    private void Start()
    {
        _currentScore = DataStorage.LoadCurrentScore();
        GameEventsScore.OnModifyCurrentScore(_currentScore);
        _higherScore = DataStorage.LoadHigherScore();
        GameEventsScore.OnModifyHigherScore(_higherScore);
        _lastTimeDisconnection = DataStorage.LoadDisconnectionDate();

        CheckPastMidnights();

        StartCoroutine(CheckScoreAtMidnight());
    }

    private void CheckHigherScore()
    {
        if(_currentScore > _higherScore)
        {
            _higherScore = _currentScore;
            GameEventsScore.OnModifyHigherScore(_higherScore);
        }

        _currentScore = 0;
        GameEventsScore.OnModifyCurrentScore(_currentScore);
    }

    private void CheckPastMidnights()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeDifference = currentTime.Date - _lastTimeDisconnection.Date;

        if(Mathf.Abs(timeDifference.Days) >= 1)
        {
            CheckHigherScore();
        }
    }

    private IEnumerator CheckScoreAtMidnight()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.AddDays(1).Date;

            yield return new WaitForSeconds((float)(nextMidnight - now).TotalSeconds);

            CheckHigherScore();
        }
    }

    public void AddScore(int score)
    {
        _currentScore += score;
        GameEventsScore.OnModifyCurrentScore(_currentScore);
    }

    public void SubstractScore(int score)
    {
        if(_currentScore - score < 0)
        {
            _currentScore = 0;
            GameEventsScore.OnModifyCurrentScore(0);
        }
        else
        {
            _currentScore -= score;
            GameEventsScore.OnModifyCurrentScore(_currentScore);
        }
    }
}
