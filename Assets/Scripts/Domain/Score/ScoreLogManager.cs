using Master.Domain.GameEvents;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
            if (scoreLogListAux.Count > 0 && scoreLogListAux[0].GetTime().Date == DateTime.Now.Date)
            {
                for (int i = 0; i < scoreLogListAux.Count; i++)
                {
                    scoreLogList.Add(scoreLogListAux[i]);
                }
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

                    // Se inserta el nuevo elemento.
                    scoreLogList.Add(newScoreLog);
                    UnityEngine.Debug.Log($"Nuevo log: {newScoreLog.GetInfo()}");

                    SaveScoreLog();
                    GameEvents_Score.OnAddScoreLog?.Invoke(newScoreLog, 0);
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public void ClearScoreLogElements()
        {
            scoreLogList.Clear();
            SaveScoreLog();
            GameEvents_Score.OnResetScoreLog?.Invoke();
        }

        private void SaveScoreLog()
        {
            _scoreRepository.SaveScoreLog(scoreLogList);
        }
    }
}