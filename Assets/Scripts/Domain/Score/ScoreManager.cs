using System;
using Master.Domain.GameEvents;
using System.Threading;
using Master.Domain.Connection;
using System.Threading.Tasks;

namespace Master.Domain.Score
{
    public class ScoreManager : IScoreManager
    {
        IScoreRepository _scoreRepository;
        IConnectionManager _connectionManager;
        IScoreLogManager _scoreLogManager;

        private Mutex _mutex = new Mutex();
        public int currentScore { private set; get; }
        public int highestScore { private set; get; }

        private DateTime _currentDate;

        public ScoreManager(IScoreRepository scoreRepository, IConnectionManager connectionManager, IScoreLogManager scoreLogManager)
        {
            _scoreRepository = scoreRepository;
            _connectionManager = connectionManager;
            _scoreLogManager = scoreLogManager;

            highestScore = _scoreRepository.LoadHighestScore();
            currentScore = _scoreRepository.LoadCurrentScore();
            _currentDate = _connectionManager.lastDisconnectionDateTime.Date;

            _ = CheckScoreAtMidnightAsync();
            _scoreLogManager = scoreLogManager;
        }

        private void CheckHighestScore(DateTime newDateTime)
        {
            _currentDate = newDateTime.Date;

            if (currentScore > highestScore)
            {
                SetHighestScore(currentScore);
            }

            ResetScore();
        }

        private async Task CheckScoreAtMidnightAsync()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime nextMidnight = now.AddDays(1).Date;
                TimeSpan timeUntilMidnight = nextMidnight - now;

                await Task.Delay(timeUntilMidnight);

                CheckHighestScore(now);
            }
        }

        public void AddScore(int addedScore, DateTime? time, string activity)
        {
            _mutex.WaitOne();
            try
            {
                SetCurrentScore(currentScore + addedScore, time);
                _scoreLogManager.AddScoreLogElement(addedScore, time, activity);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void SubstractScore(int substractedScore, DateTime? time, string activity)
        {
            _mutex.WaitOne();
            try
            {
                if (currentScore - substractedScore < 0)
                {
                    SetCurrentScore(0, time);
                }
                else
                {
                    SetCurrentScore(currentScore - substractedScore, time);
                }
                _scoreLogManager.AddScoreLogElement(-substractedScore, time, activity);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ResetScore()
        {
            _mutex.WaitOne();
            try
            {
                currentScore = 0;
                _scoreRepository.SaveCurrentScore(currentScore);
                GameEvents_Score.OnResetScore?.Invoke();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private void SetCurrentScore(int newCurrentScore, DateTime? time)
        {
            if (time != null && _currentDate.Date < time.Value.Date)
            {
                CheckHighestScore(time.Value);
            }

            int addedScore = newCurrentScore - currentScore;
            currentScore = newCurrentScore;
            _scoreRepository.SaveCurrentScore(currentScore);
            GameEvents_Score.OnModifyCurrentScore?.Invoke(currentScore);
        }

        private void SetHighestScore(int newHighestScore)
        {
            highestScore = newHighestScore;
            _scoreRepository.SaveHighestScore(highestScore);
            GameEvents_Score.OnModifyHighestScore?.Invoke(highestScore);
        }
    }
}