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

        public void SaveScoreInfo(List<ScoreLogData> infoList)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "ScoreLogData.txt");

            ScoreLogDataList allScoreElements = new ScoreLogDataList();
            foreach (ScoreLogData scoreData in infoList)
            {
                allScoreElements.score.Add(scoreData);
            }

            string json = JsonUtility.ToJson(allScoreElements, true);

            using (StreamWriter streamWriter = new StreamWriter(path, false))
            {
                streamWriter.Write(json);
            }
        }

        public List<ScoreLogData> LoadScoreInfo()
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "ScoreLogData.txt");

            if (!File.Exists(path))
                return new List<ScoreLogData>();

            string existingJson = null;

            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            ScoreLogDataList allScoreElements = JsonUtility.FromJson<ScoreLogDataList>(existingJson);

            return allScoreElements.score;
        }
    }
}