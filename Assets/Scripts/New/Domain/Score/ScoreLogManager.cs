using Master.Domain.GameEvents;
using Master.Persistence.Score;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Domain.Score
{
    public class ScoreLogManager
    {
        public List<ScoreLog> scoreLogList { private set; get; }

        private readonly Action<IEnumerator> _coroutineRunner;

        public ScoreLogManager(Action<IEnumerator> coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;

            scoreLogList = new List<ScoreLog>();
            List<ScoreLog> scoreLogListAux = DataStorage_Score.LoadScoreLog();
            if (scoreLogListAux.Count > 0 && scoreLogListAux[0].GetTime() == DateTime.Now.Date)
            {
                scoreLogList = scoreLogListAux;
            }

            _coroutineRunner?.Invoke(CheckScoreAtMidnight());
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

                ClearScoreLogElements();

                break;
            }
        }

        public void AddScoreLogElement(int addedScoreWithSign, DateTime? time, string activity)
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

        public void ClearScoreLogElements()
        {
            scoreLogList.Clear();
            SaveScoreLog();
            GameEvents_Score.OnResetScoreLog?.Invoke();
        }

        private void SaveScoreLog()
        {
            DataStorage_Score.SaveScoreLog(scoreLogList);
        }
    }
}