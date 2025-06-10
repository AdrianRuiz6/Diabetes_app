using Master.Domain.GameEvents;
using Master.Persistence.Score;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Master.Domain.Score
{
    public interface IScoreLogManager
    {
        public List<ScoreLog> scoreLogList { get; }
        public void AddScoreLogElement(int addedScoreWithSign, DateTime? time, string activity);

        public void ClearScoreLogElements();
    }
}