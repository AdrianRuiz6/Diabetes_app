using System;
using System.Collections.Generic;
using Master.Domain.Settings;
using Master.Domain.PetCare;
using Master.Domain.Connection;

namespace Master.Domain.BehaviorTree
{
    public enum TreeType
    {
        Glycemia,
        Activity,
        Hunger
    }

    public abstract class BTree
    {
        private Node _root;
        private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

        protected TreeType _treeType;
        protected IPetCareManager _petCareManager;
        protected ISettingsManager _settingsManager;
        protected IConnectionManager _connectionManager;

        public BTree(IPetCareManager petCareManager, ISettingsManager settingsManager, IConnectionManager connectionManager)
        {
            _petCareManager = petCareManager;
            _settingsManager = settingsManager;
            _connectionManager = connectionManager;

            _root = SetUpTree();
        }

        public void EnqueueAttribute(DateTime currentDateTime)
        {
            bool IsInTime = _settingsManager.IsInRange(currentDateTime.TimeOfDay);
            bool IsInCurrentDate = currentDateTime.Date == _connectionManager.currentConnectionDateTime.Date;
            bool IsInLastSessionDate = currentDateTime.Date == _connectionManager.lastDisconnectionDateTime.Date;

            if (IsInLastSessionDate && IsInTime || IsInCurrentDate && IsInTime)
            {
                _dateTimesQueue.Enqueue(currentDateTime);
            }
        }

        public void Run()
        {
            while (_dateTimesQueue.Count > 0)
            {
                DateTime nextDateTime = _dateTimesQueue.Peek();

                if (_dateTimesQueue.Peek().TimeOfDay == _settingsManager.initialTime)
                {
                    switch (_treeType)
                    {
                        case TreeType.Glycemia:
                            _petCareManager.RestartGlycemia(nextDateTime);
                            break;
                        case TreeType.Activity:
                            _petCareManager.RestartActivity(nextDateTime);
                            break;
                        case TreeType.Hunger:
                            _petCareManager.RestartHunger(nextDateTime);
                            break;
                    }
                }
                else
                {
                    NodeState result = _root.Evaluate(nextDateTime);

                    while (result == NodeState.RUNNING)
                    {
                        result = _root.Evaluate(nextDateTime);
                    }
                }

                _dateTimesQueue.Dequeue();
            }
        }

        protected abstract Node SetUpTree();
    }
}
