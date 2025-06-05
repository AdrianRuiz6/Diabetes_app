using System;

namespace Master.Domain.Connection
{
    public interface IConnectionRepository
    {
        public void SaveIsFirstUsage(bool newIsFirstUsage);

        public bool LoadIsFirstUsage();

        public void SaveDisconnectionDate(DateTime disconnectionDate);

        public DateTime LoadDisconnectionDate();
    }
}