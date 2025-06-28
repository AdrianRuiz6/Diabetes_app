using Master.Domain.PetCare;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace Master.Persistence.PetCare
{
    public class DataStorage_PetCare : IPetCareRepository
    {
        #region Log

        #region AttributeLog
        public void SaveGlycemiaLog(List<AttributeLog> glycemiaLogList)
        {
            string path = $"{Application.persistentDataPath}/GlycemiaLogData.txt";
            SaveAttributeLog(path, glycemiaLogList);
        }

        public List<AttributeLog> LoadGlycemiaLog()
        {
            string path = $"{Application.persistentDataPath}/GlycemiaLogData.txt";
            return LoadAttributeLog(path);
        }

        public void SaveHungerLog(List<AttributeLog> hungerLogList)
        {
            string path = $"{Application.persistentDataPath}/HungerLogData.txt";
            SaveAttributeLog(path, hungerLogList);
        }

        public List<AttributeLog> LoadHungerLog()
        {
            string path = $"{Application.persistentDataPath}/HungerLogData.txt";
            return LoadAttributeLog(path);
        }

        public void SaveEnergyLog(List<AttributeLog> energyLogList)
        {
            string path = $"{Application.persistentDataPath}/EnergyLogData.txt";
            SaveAttributeLog(path, energyLogList);
        }

        public List<AttributeLog> LoadEnergyLog()
        {
            string path = $"{Application.persistentDataPath}/EnergyLogData.txt";
            return LoadAttributeLog(path);
        }

        private void SaveAttributeLog(string path, List<AttributeLog> attributeLogList)
        {
            AttributeLogDataList attributeLogDataList = new AttributeLogDataList();

            foreach (AttributeLog attributeLog in attributeLogList)
            {
                attributeLogDataList.attributeLogList.Add(new AttributeLogData(attributeLog.GetDateAndTime(), attributeLog.GetValue()));
            }

            string json = JsonUtility.ToJson(attributeLogDataList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private List<AttributeLog> LoadAttributeLog(string path)
        {
            List<AttributeLog> result = new List<AttributeLog>();
            if (!File.Exists(path))
            {
                return result;
            }

            string existingJson = null;
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            AttributeLogDataList attributeLogDataList = JsonUtility.FromJson<AttributeLogDataList>(existingJson);

            foreach (AttributeLogData attributeLogData in attributeLogDataList.attributeLogList)
            {
                result.Add(new AttributeLog(attributeLogData.GetDateAndTime(), attributeLogData.GetValue()));
            }

            return result;
        }
        #endregion

        #region ActionsLog
        public void SaveInsulinLog(List<ActionLog> insulinLogList)
        {
            string path = $"{Application.persistentDataPath}/InsulinLogData.txt";
            SaveActionLog(path, insulinLogList);
        }

        public List<ActionLog> LoadInsulinLog()
        {
            string path = $"{Application.persistentDataPath}/InsulinLogData.txt";
            return LoadActionLog(path);
        }
        public void SaveFoodLog(List<ActionLog> foodLogList)
        {
            string path = $"{Application.persistentDataPath}/FoodLogData.txt";
            SaveActionLog(path, foodLogList);
        }

        public List<ActionLog> LoadFoodLog()
        {
            string path = $"{Application.persistentDataPath}/FoodLogData.txt";
            return LoadActionLog(path);
        }

        public void SaveExerciseLog(List<ActionLog> exerciseLogList)
        {
            string path = $"{Application.persistentDataPath}/ExerciseLogData.txt";
            SaveActionLog(path, exerciseLogList);
        }

        public List<ActionLog> LoadExerciseLog()
        {
            string path = $"{Application.persistentDataPath}/ExerciseLogData.txt";
            return LoadActionLog(path);
        }

        private void SaveActionLog(string path, List<ActionLog> actionLogList)
        {
            ActionLogDataList actionLogDataList = new ActionLogDataList();

            foreach (ActionLog actionLog in actionLogList)
            {
                actionLogDataList.actionsLogList.Add(new ActionLogData(actionLog.GetDateAndTime(), actionLog.GetInformation()));
            }

            string json = JsonUtility.ToJson(actionLogDataList, true);
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(json);
            }
        }

        private List<ActionLog> LoadActionLog(string path)
        {
            List<ActionLog> result = new List<ActionLog>();
            if (!File.Exists(path))
            {
                return result;
            }

            string existingJson = null;
            using (StreamReader streamReader = new StreamReader(path))
            {
                existingJson = streamReader.ReadToEnd();
            }

            ActionLogDataList actionLogDataList = JsonUtility.FromJson<ActionLogDataList>(existingJson);
            foreach (ActionLogData actionLogData in actionLogDataList.actionsLogList)
            {
                result.Add(new ActionLog(actionLogData.GetDateAndTime(), actionLogData.GetInformation()));
            }

            return result;
        }
        #endregion
        #endregion

        #region Actions
        public void SaveInsulinCooldownEndTime(DateTime insulinCooldownEndTime)
        {
            PlayerPrefs.SetString("InsulinCooldownEndTime", insulinCooldownEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadInsulinCooldownEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("InsulinCooldownEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }

        public void SaveInsulinEffectsEndTime(DateTime insulinEffectsEndTime)
        {
            PlayerPrefs.SetString("InsulinEffectsEndTime", insulinEffectsEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadInsulinEffectsEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("InsulinEffectsEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }

        public void SaveExerciseCooldownEndTime(DateTime exerciseCooldownEndTime)
        {
            PlayerPrefs.SetString("ExerciseCooldownEndTime", exerciseCooldownEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadExerciseCooldownEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("ExerciseCooldownEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }

        public void SaveExerciseEffectsEndTime(DateTime exerciseEffectsEndTime)
        {
            PlayerPrefs.SetString("ExerciseEffectsEndTime", exerciseEffectsEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadExerciseEffectsEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("ExerciseEffectsEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }

        public void SaveFoodCooldownEndTime(DateTime foodCooldownEndTime)
        {
            PlayerPrefs.SetString("FoodCooldownEndTime", foodCooldownEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadFoodCooldownEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("FoodCooldownEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }

        public void SaveFoodEffectsEndTime(DateTime foodEffectsEndTime)
        {
            PlayerPrefs.SetString("FoodEffectsEndTime", foodEffectsEndTime.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadFoodEffectsEndTime()
        {
            String timeSaved = PlayerPrefs.GetString("FoodEffectsEndTime", string.Empty);
            if (timeSaved != string.Empty)
            {
                return DateTime.Parse(timeSaved);
            }

            return DateTime.Now.AddSeconds(-1);
        }
        #endregion

        #region Attributes
        public void SaveNextIterationStartTime(DateTime startTimeInterval)
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
            return PlayerPrefs.GetInt("Glycemia", 120);
        }

        public void SaveEnergy(int energyValue)
        {
            PlayerPrefs.SetInt("Energy", energyValue);
            PlayerPrefs.Save();
        }

        public int LoadEnergy()
        {
            return PlayerPrefs.GetInt("Energy", 50);
        }

        public void SaveHunger(int hungerValue)
        {
            PlayerPrefs.SetInt("Hunger", hungerValue);
            PlayerPrefs.Save();
        }

        public int LoadHunger()
        {
            return PlayerPrefs.GetInt("Hunger", 50);
        }
        #endregion
    }
}