using Master.Persistence.Score;
using System.Collections.Generic;

namespace Master.Domain.Score
{
    public interface IScoreRepository
    {
        public void SaveCurrentScore(int currentScore);

        public int LoadCurrentScore();

        public void SaveHighestScore(int currentScore);

        public int LoadHighestScore();

        public void SaveScoreInfo(List<ScoreLogData> infoList);

        public List<ScoreLogData> LoadScoreInfo();
    }
}