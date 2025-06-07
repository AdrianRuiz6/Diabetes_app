using System;
using System.Collections;
using UnityEngine;
using Master.Domain.GameEvents;
using Master.Persistence.Connection;
using Master.Persistence.Score;

namespace Master.Domain.Score
{
    public class ScoreManager
    {
        public int currentScore { private set; get; }
        public int highestScore { private set; get; }

        private readonly Action<IEnumerator> _coroutineRunner;
        private DateTime _currentDate;

        public ScoreManager(Action<IEnumerator> coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;

            highestScore = DataStorage_Score.LoadHighestScore();
            currentScore = DataStorage_Score.LoadCurrentScore();
            _currentDate = DataStorage_Connection.LoadDisconnectionDate().Date;
        }

        public void Initialize()
        {
            if (_currentDate < DateTime.Now.Date)
            {
                CheckHighestScore();
            }

            _coroutineRunner?.Invoke(CheckScoreAtMidnight());
        }

        private void CheckHighestScore()
        {
            _currentDate = DateTime.Now.Date;

            if (currentScore > highestScore)
            {
                SetHighestScore(currentScore);
            }

            ResetScore();
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

                break;
            }
        }

        public void AddScore(int addedScore)
        {
            SetCurrentScore(currentScore + addedScore);
        }

        public void SubstractScore(int substractedScore)
        {
            if (currentScore - substractedScore < 0)
            {
                SetCurrentScore(0);
            }
            else
            {
                SetCurrentScore(currentScore - substractedScore);
            }
        }

        public void ResetScore()
        {
            SetCurrentScore(0);
            GameEvents_Score.OnResetScore?.Invoke();
        }

        private void SetCurrentScore(int newCurrentScore)
        {
            if (time != null && _currentDate < time.Value.Date)
            {
                CheckHighestScore();
            }
            else
            {
                int addedScore = newCurrentScore - currentScore;
                currentScore = newCurrentScore;
                DataStorage_Score.SaveCurrentScore(currentScore);
                GameEvents_Score.OnModifyCurrentScore?.Invoke(currentScore);
            }
        }

        private void SetHighestScore(int newHighestScore)
        {
            highestScore = newHighestScore;
            DataStorage_Score.SaveHighestScore(highestScore);
            GameEvents_Score.OnModifyHighestScore?.Invoke(highestScore);
        }
    }
}