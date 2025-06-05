using System;
using System.Collections;
using UnityEngine;
using Master.Persistence;
using Master.Domain.GameEvents;
using Master.Persistence.Connection;
using Master.Persistence.Score;
using JetBrains.Annotations;

namespace Master.Domain.Score
{
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

        private void Start()
        {
            _highestScore = DataStorage_Score.LoadHighestScore();
            _currentScore = DataStorage_Score.LoadCurrentScore();

            if (DataStorage_Connection.LoadDisconnectionDate().Date < DateTime.Now.Date)
            {
                CheckHighestScore();
                SetCurrentScore(0);
            }

            StartCoroutine(CheckScoreAtMidnight());
        }

        private void CheckHighestScore()
        {
            if (_currentScore > _highestScore)
            {
                SetHighestScore(_currentScore);
                GameEvents_Score.OnModifyHighestScore?.Invoke(_highestScore);
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
                SetCurrentScore(0);
                GameEvents_Score.OnResetScore?.Invoke();
                break;
            }
        }

        public void AddScore(int score, DateTime? time, string activity)
        {
            SetCurrentScore(_currentScore + score);
            GameEvents_Score.OnModifyCurrentScore?.Invoke(score, time, activity);
        }

        public void SubstractScore(int score, DateTime? time, string activity)
        {
            if (_currentScore - score < 0)
            {
                SetCurrentScore(0);
                GameEvents_Score.OnModifyCurrentScore?.Invoke(-score, time, activity);
            }
            else
            {
                SetCurrentScore(_currentScore - score);
                GameEvents_Score.OnModifyCurrentScore?.Invoke(-score, time, activity);
            }
        }

        public void ResetScore()
        {
            GameEvents_Score.OnResetScore?.Invoke();
            SetCurrentScore(0);
        }

        private void SetCurrentScore(int newCurrentScore)
        {
            _currentScore = newCurrentScore;
            DataStorage_Score.SaveCurrentScore(_currentScore);
            
        }

        private void SetHighestScore(int newHighestScore)
        {
            _highestScore = newHighestScore;
            DataStorage_Score.SaveHighestScore(_highestScore);
        }
    }
}