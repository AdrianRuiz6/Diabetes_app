using System;
using UnityEngine;

namespace Master.Persistence.Connection
{
    public static class DataStorage_Connection
    {
        public static void SaveIsFirstUsage()
        {
            PlayerPrefs.SetInt("IsFirstUsage", 1);
            PlayerPrefs.Save();
        }

        public static bool LoadIsFirstUsage()
        {
            int firstUsage = PlayerPrefs.GetInt("IsFirstUsage", 0);

            if (firstUsage == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void SaveDisconnectionDate()
        {
            PlayerPrefs.SetString("DisconnectionDate", DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        public static DateTime LoadDisconnectionDate()
        {
            return DateTime.Parse(PlayerPrefs.GetString("DisconnectionDate", DateTime.Now.ToString()));
        }

        public static void SaveLastIterationStartTime(DateTime startTimeInterval)
        {
            PlayerPrefs.SetString("LastIntervalStartTime", startTimeInterval.ToString());
            PlayerPrefs.Save();
        }

        public static DateTime LoadLastIterationStartTime()
        {
            return DateTime.Parse(PlayerPrefs.GetString("LastIntervalStartTime", DateTime.Now.Date.ToString()));
        }
    }
}