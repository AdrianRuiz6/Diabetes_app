using Master.Persistence.Score;
using System;

namespace Master.Domain.GameEvents
{
    public static class GameEvents_Score
    {
        public static Action<int> OnModifyCurrentScore;
        public static Action<int> OnModifyHighestScore;
        public static Action OnResetScore;

        public static Action<ScoreLog, int> OnAddScoreLog;
        public static Action OnResetScoreLog;
    }
}