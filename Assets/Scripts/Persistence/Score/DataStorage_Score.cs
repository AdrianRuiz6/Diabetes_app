using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Master.Domain.Score;

namespace Master.Persistence.Score
{
    public class DataStorage_Score : IScoreRepository
    {
        public void SaveCurrentScore(int currentScore)
        {
            PlayerPrefs.SetInt("CurrentScore", currentScore);
            PlayerPrefs.Save();
        }

        public int LoadCurrentScore()
        {
            return PlayerPrefs.GetInt("CurrentScore", 0);
        }

        public void SaveHighestScore(int currentScore)
        {
            PlayerPrefs.SetInt("HigherScore", currentScore);
            PlayerPrefs.Save();
        }

        public int LoadHighestScore()
        {
            return PlayerPrefs.GetInt("HigherScore", 0);
        }

        public void SaveScoreLog(List<ScoreLog> scoreLogList)
        {
            string path = Path.Combine(Application.persistentDataPath, "ScoreLogData.txt");

            ScoreLogDataList allScoreElements = new ScoreLogDataList();
            foreach (ScoreLog scoreLog in scoreLogList)
            {
                allScoreElements.scoreLogDataList.Add(new ScoreLogData(scoreLog.GetTime(), scoreLog.GetInfo()));
            }

            string json = JsonUtility.ToJson(allScoreElements, true);

            using (StreamWriter streamWriter = new StreamWriter(path, false))
            {
                streamWriter.Write(json);
            }
        }

        public List<ScoreLog> LoadScoreLog()
        {
            string path = Path.Combine(Application.persistentDataPath, "ScoreLogData.txt");
            List<ScoreLog> result = new List<ScoreLog>();

            if (!File.Exists(path))
                return result;

            string existingJson = null;

            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            ScoreLogDataList allScoreElements = JsonUtility.FromJson<ScoreLogDataList>(existingJson);
            foreach (ScoreLogData scoreLogData in allScoreElements.scoreLogDataList)
            {
                result.Add(new ScoreLog(scoreLogData.GetTime(), scoreLogData.GetInfo()));
            }

            return result;
        }
    }
}