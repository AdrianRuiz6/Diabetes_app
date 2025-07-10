using System.Collections.Generic;

namespace Master.Domain.Score
{
    public interface IScoreRepository
    {
        public void SaveCurrentScore(int currentScore);

        public int LoadCurrentScore();

        public void SaveHighestScore(int currentScore);

        public int LoadHighestScore();

        public void SaveScoreLog(List<ScoreLog> scoreLogList);

        public List<ScoreLog> LoadScoreLog();
    }
}