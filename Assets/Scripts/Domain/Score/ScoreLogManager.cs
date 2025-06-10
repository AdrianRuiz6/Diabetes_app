using Master.Domain.GameEvents;
using Master.Persistence.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Master.Domain.Score
{
    public class ScoreLogManager : IScoreLogManager
    {
        IScoreRepository _scoreRepository;
        private Mutex _mutex = new Mutex();
        public List<ScoreLog> scoreLogList { private set; get; }

        public ScoreLogManager(IScoreRepository scoreRepository)
        {
            _scoreRepository = scoreRepository;

            scoreLogList = new List<ScoreLog>();
            List<ScoreLog> scoreLogListAux = _scoreRepository.LoadScoreLog();
            if (scoreLogListAux.Count > 0 && scoreLogListAux[0].GetTime() == DateTime.Now.Date)
            {
                scoreLogList = scoreLogListAux;
            }

            _ = CheckScoreAtMidnightAsync();
        }

        private async Task CheckScoreAtMidnightAsync()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime nextMidnight = now.AddDays(1).Date;
                TimeSpan timeUntilMidnight = nextMidnight - now;

                await Task.Delay(timeUntilMidnight);

                ClearScoreLogElements();
            }
        }

        public void AddScoreLogElement(int addedScoreWithSign, DateTime? time, string activity)
        {
            _mutex.WaitOne();
            try
            {
                if (time.Value.Date == DateTime.Now.Date)
                {
                    string sign = (addedScoreWithSign >= 0) ? "+" : "";
                    string info = $"{sign}{addedScoreWithSign} por {activity}.";

                    ScoreLog newScoreLog = new ScoreLog(time.Value, info);

                    // Se inserta el nuevo elemento en la posición correcta dentro de scoreLogList.
                    int index = scoreLogList.FindLastIndex(log => log.GetTime() <= time.Value) + 1;
                    if (index == -1)
                    {
                        scoreLogList.Add(newScoreLog);
                    }
                    else
                    {
                        scoreLogList.Insert(index, newScoreLog);
                    }

                    int siblingIndex = scoreLogList.Count - index - 1;

                    SaveScoreLog();
                    GameEvents_Score.OnAddScoreLog?.Invoke(newScoreLog, siblingIndex);
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ClearScoreLogElements()
        {
            _mutex.WaitOne();
            try
            {
                scoreLogList.Clear();
                SaveScoreLog();
                GameEvents_Score.OnResetScoreLog?.Invoke();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private void SaveScoreLog()
        {
            _scoreRepository.SaveScoreLog(scoreLogList);
        }
    }
}