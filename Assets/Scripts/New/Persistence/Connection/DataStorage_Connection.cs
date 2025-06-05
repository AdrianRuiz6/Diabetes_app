using Master.Domain.Connection;
using System;
using UnityEngine;

namespace Master.Persistence.Connection
{
    public class DataStorage_Connection : IConnectionRepository
    {
        public void SaveIsFirstUsage(bool newIsFirstUsage)
        {
            int firstUsage = 0;

            if (newIsFirstUsage)
                firstUsage = 0;
            else
                firstUsage = 1;

            PlayerPrefs.SetInt("IsFirstUsage", firstUsage);
            PlayerPrefs.Save();
        }

        public bool LoadIsFirstUsage()
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

        public void SaveDisconnectionDate(DateTime disconnectionDate)
        {
            PlayerPrefs.SetString("DisconnectionDate", disconnectionDate.ToString());
            PlayerPrefs.Save();
        }

        public DateTime LoadDisconnectionDate()
        {
            return DateTime.Parse(PlayerPrefs.GetString("DisconnectionDate", DateTime.Now.ToString()));
        }
    }
}