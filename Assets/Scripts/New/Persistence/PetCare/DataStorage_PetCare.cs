using Master.Domain.PetCare;
using Master.Domain.Settings;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    public class DataStorage_PetCare : IPetCareRepository
    {
        #region Log

        #region ButtonLog
        public void SaveInsulinLog(DateTime? dateTime, string information)
        {
            string path = $"{Application.persistentDataPath}/InsulinLogData.txt";
            SaveButtonLog(dateTime, information, path);
        }

        public Dictionary<DateTime, string> LoadInsulinLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/InsulinLogData.txt";
            return LoadButtonLog(path, requestedDate);
        }
        public void SaveFoodLog(DateTime? dateTime, string information)
        {
            string path = $"{Application.persistentDataPath}/FoodLogData.txt";
            SaveButtonLog(dateTime, information, path);
        }

        public Dictionary<DateTime, string> LoadFoodLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/FoodLogData.txt";
            return LoadButtonLog(path, requestedDate);
        }

        public void SaveExerciseLog(DateTime? dateTime, string information)
        {
            string path = $"{Application.persistentDataPath}/ExerciseLogData.txt";
            SaveButtonLog(dateTime, information, path);
        }

        public Dictionary<DateTime, string> LoadExerciseLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/ExerciseLogData.txt";
            return LoadButtonLog(path, requestedDate);
        }

        public void ResetInsulinLog()
        {
            string path = $"{Application.persistentDataPath}/InsulinLogData.txt";
            ResetActionsLog(path);
        }

        public void ResetFoodLog()
        {
            string path = $"{Application.persistentDataPath}/FoodLogData.txt";
            ResetActionsLog(path);
        }

        public void ResetExerciseLog()
        {
            string path = $"{Application.persistentDataPath}/ExerciseLogData.txt";
            ResetActionsLog(path);
        }

        private void ResetActionsLog(string path)
        {
            ButtonLogDataList originalButtonList = new ButtonLogDataList();
            ButtonLogDataList newButtonList = new ButtonLogDataList();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                originalButtonList = JsonUtility.FromJson<ButtonLogDataList>(existingJson) ?? new ButtonLogDataList();
            }

            foreach (ButtonLogData buttonData in originalButtonList.buttonList)
            {
                DateTime currentDate = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                if (currentDate.Date != DateTime.Now.Date)
                {
                    newButtonList.buttonList.Add(buttonData);
                }
            }

            string json = JsonUtility.ToJson(newButtonList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private void SaveButtonLog(DateTime? dateTime, string information, string path)
        {
            ButtonLogDataList originalButtonList = new ButtonLogDataList();
            ButtonLogDataList newButtonList = new ButtonLogDataList();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                originalButtonList = JsonUtility.FromJson<ButtonLogDataList>(existingJson) ?? new ButtonLogDataList();
            }

            foreach (ButtonLogData buttonData in originalButtonList.buttonList)
            {
                // DateTime currentDateAndTime = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                newButtonList.buttonList.Add(buttonData);
            }

            ButtonLogData newButtonData = new ButtonLogData(dateTime, information);
            newButtonList.buttonList.Add(newButtonData);

            string json = JsonUtility.ToJson(newButtonList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private Dictionary<DateTime, string> LoadButtonLog(string path, DateTime? requestedDate)
        {
            if (!File.Exists(path))
            {
                return new Dictionary<DateTime, string>();
            }

            string existingJson = null;
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            ButtonLogDataList buttonDataList = JsonUtility.FromJson<ButtonLogDataList>(existingJson);

            Dictionary<DateTime, string> askedDateButtonDictionary = new Dictionary<DateTime, string>();
            foreach (ButtonLogData buttonData in buttonDataList.buttonList)
            {
                DateTime currentDate = DateTime.Parse(buttonData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                if (requestedDate.Value.Date == currentDate.Date && SettingsManager.Instance.IsInRange(currentDate.TimeOfDay))
                {
                    askedDateButtonDictionary.Add(currentDate, buttonData.Information);
                }
            }

            return askedDateButtonDictionary;
        }
        #endregion

        #region AttributeLog
        public void SaveGlycemiaLog(DateTime? dateTime, int number)
        {
            string path = $"{Application.persistentDataPath}/GlycemiaLogData.txt";
            SaveAttributeLog(dateTime, number, path);
        }
        public void ResetGlycemiaLog()
        {
            string path = $"{Application.persistentDataPath}/GlycemiaLogData.txt";
            ResetAttributeLog(path);
        }
        public Dictionary<DateTime, int> LoadGlycemiaLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/GlycemiaLogData.txt";
            return LoadAttributeLog(path, requestedDate);
        }

        public void SaveHungerLog(DateTime? dateTime, int number)
        {
            string path = $"{Application.persistentDataPath}/HungerLogData.txt";
            SaveAttributeLog(dateTime, number, path);
        }
        public void ResetHungerLog()
        {
            string path = $"{Application.persistentDataPath}/HungerLogData.txt";
            ResetAttributeLog(path);
        }
        public Dictionary<DateTime, int> LoadHungerLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/HungerLogData.txt";
            return LoadAttributeLog(path, requestedDate);
        }

        public void SaveActivityLog(DateTime? dateTime, int number)
        {
            string path = $"{Application.persistentDataPath}/ActivityLogData.txt";
            SaveAttributeLog(dateTime, number, path);
        }
        public void ResetActivityLog()
        {
            string path = $"{Application.persistentDataPath}/ActivityLogData.txt";
            ResetAttributeLog(path);
        }
        public Dictionary<DateTime, int> LoadActivityLog(DateTime? requestedDate)
        {
            string path = $"{Application.persistentDataPath}/ActivityLogData.txt";
            return LoadAttributeLog(path, requestedDate);
        }

        private void SaveAttributeLog(DateTime? dateTime, int number, string path)
        {
            AttributeLogDataList originalAttributeList = new AttributeLogDataList();
            AttributeLogDataList newAttributeList = new AttributeLogDataList();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                originalAttributeList = JsonUtility.FromJson<AttributeLogDataList>(existingJson) ?? new AttributeLogDataList();
            }

            foreach (AttributeLogData attributeData in originalAttributeList.attributeList)
            {
                newAttributeList.attributeList.Add(attributeData);
            }

            AttributeLogData newAttributeData = new AttributeLogData(dateTime, number);
            newAttributeList.attributeList.Add(newAttributeData);

            string json = JsonUtility.ToJson(newAttributeList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private void ResetAttributeLog(string path)
        {
            AttributeLogDataList originalAttributeList = new AttributeLogDataList();
            AttributeLogDataList newAttributeList = new AttributeLogDataList();

            if (File.Exists(path))
            {
                string existingJson = File.ReadAllText(path);
                originalAttributeList = JsonUtility.FromJson<AttributeLogDataList>(existingJson) ?? new AttributeLogDataList();
            }

            foreach (AttributeLogData attributeData in originalAttributeList.attributeList)
            {
                DateTime currentDate = DateTime.Parse(attributeData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                if (currentDate.Date != DateTime.Now.Date)
                {
                    newAttributeList.attributeList.Add(attributeData);
                }
            }

            string json = JsonUtility.ToJson(newAttributeList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private Dictionary<DateTime, int> LoadAttributeLog(string path, DateTime? requestedDate)
        {
            if (!File.Exists(path))
            {
                return new Dictionary<DateTime, int>();
            }

            string existingJson = null;
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            AttributeLogDataList attributeDataList = JsonUtility.FromJson<AttributeLogDataList>(existingJson);

            Dictionary<DateTime, int> askedDateAttributeDictionary = new Dictionary<DateTime, int>();
            foreach (AttributeLogData attributeData in attributeDataList.attributeList)
            {
                DateTime currentDate = DateTime.Parse(attributeData.DateAndTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                if (requestedDate.Value.Date == currentDate.Date && SettingsManager.Instance.IsInRange(currentDate.TimeOfDay))
                {
                    askedDateAttributeDictionary.Add(currentDate, attributeData.Value);
                }
            }

            return askedDateAttributeDictionary;
        }
        #endregion
        #endregion

        #region Buttons
        public void SaveLastTimeInsulinUsed(DateTime? lastTimeInsulinUsed)
        {
            PlayerPrefs.SetString("LastTimeInsulinUsed", lastTimeInsulinUsed.ToString());
            PlayerPrefs.Save();
        }

        public DateTime? LoadLastTimeInsulinUsed()
        {
            String timeSaved = PlayerPrefs.GetString("LastTimeInsulinUsed", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return null;
        }

        public void SaveLastTimeExerciseUsed(DateTime? lastTimeExerciseUsed)
        {
            PlayerPrefs.SetString("LastTimeExerciseUsed", lastTimeExerciseUsed.ToString());
            PlayerPrefs.Save();
        }

        public DateTime? LoadLastTimeExerciseUsed()
        {
            String timeSaved = PlayerPrefs.GetString("LastTimeExerciseUsed", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return null;
        }

        public void SaveLastTimeFoodUsed(DateTime? lastTimeFoodUsed)
        {
            PlayerPrefs.SetString("LastTimeFoodUsed", lastTimeFoodUsed.ToString());
            PlayerPrefs.Save();
        }

        public DateTime? LoadLastTimeFoodUsed()
        {
            String timeSaved = PlayerPrefs.GetString("LastTimeFoodUsed", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return null;
        }
        #endregion

        #region Attributes
        public void SaveLastIterationStartTime(DateTime startTimeInterval)
        {
            PlayerPrefs.SetString("LastIntervalStartTime", startTimeInterval.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadLastIterationStartTime()
        {
            return DateTime.Parse(PlayerPrefs.GetString("LastIntervalStartTime", DateTime.Now.Date.ToString()));
        }

        public void SaveGlycemia(int glycemiaValue)
        {
            PlayerPrefs.SetInt("Glycemia", glycemiaValue);
            PlayerPrefs.Save();
        }

        public int LoadGlycemia()
        {
            return PlayerPrefs.GetInt("Glycemia", AttributeManager.Instance.initialGlycemiaValue);
        }

        public void SaveActivity(int activityValue)
        {
            PlayerPrefs.SetInt("Activity", activityValue);
            PlayerPrefs.Save();
        }

        public int LoadActivity()
        {
            return PlayerPrefs.GetInt("Activity", AttributeManager.Instance.initialActivityValue);
        }

        public void SaveHunger(int hungerValue)
        {
            PlayerPrefs.SetInt("Hunger", hungerValue);
            PlayerPrefs.Save();
        }

        public int LoadHunger()
        {
            return PlayerPrefs.GetInt("Hunger", AttributeManager.Instance.initialHungerValue);
        }
        #endregion
    }
}