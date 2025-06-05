using System;
using Master.Persistence.Connection;

namespace Master.Domain.Connection
{
    public class ConnectionManager
    {
        public DateTime lastDisconnectionDateTime { private set; get; }
        public DateTime currentConnectionDateTime { private set; get; }
        public bool isFirstUsage { private set; get; }

        public ConnectionManager()
        {
            lastDisconnectionDateTime = DataStorage_Connection.LoadDisconnectionDate();
            currentConnectionDateTime = DateTime.Now;

            isFirstUsage = DataStorage_Connection.LoadIsFirstUsage();
        }

        public bool IsConnected(DateTime dateTimeToEvaluate)
        {
            if (dateTimeToEvaluate > lastDisconnectionDateTime && dateTimeToEvaluate < currentConnectionDateTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetIsFirstUsage(bool newIsFirstUsage)
        {
            isFirstUsage = newIsFirstUsage;
            DataStorage_Connection.SaveIsFirstUsage(newIsFirstUsage);
        }

        public void SetDisconnectionDate(DateTime newDisconnectionDate)
        {
            DataStorage_Connection.SaveDisconnectionDate(newDisconnectionDate);
        }
    }
}