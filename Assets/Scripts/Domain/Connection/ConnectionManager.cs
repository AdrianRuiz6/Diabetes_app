using System;
using Master.Persistence.Connection;

namespace Master.Domain.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        public DateTime lastDisconnectionDateTime { get; private set; }
        public DateTime currentConnectionDateTime { get; private set; }
        public bool isFirstUsage { get; private set; }

        private IConnectionRepository _connectionRepository;

        public ConnectionManager(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;

            lastDisconnectionDateTime = _connectionRepository.LoadDisconnectionDate();
            currentConnectionDateTime = DateTime.Now;

            isFirstUsage = _connectionRepository.LoadIsFirstUsage();
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
            _connectionRepository.SaveIsFirstUsage(newIsFirstUsage);
        }

        public void SetDisconnectionDate(DateTime newDisconnectionDate)
        {
            _connectionRepository.SaveDisconnectionDate(newDisconnectionDate);
        }
    }
}