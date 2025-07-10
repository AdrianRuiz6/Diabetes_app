using System;

namespace Master.Domain.Connection
{
    public interface IConnectionManager
    {
        public DateTime lastDisconnectionDateTime { get; }
        public DateTime currentConnectionDateTime { get; }
        public bool isFirstUsage { get; }
        public bool IsConnected(DateTime dateTimeToEvaluate);

        public void SetIsFirstUsage(bool newIsFirstUsage);

        public void SetDisconnectionDate(DateTime newDisconnectionDate);
    }
}