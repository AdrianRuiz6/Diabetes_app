using System;
using System.Collections.Generic;
using Master.Domain.Settings;
using Master.Domain.PetCare;
using Master.Domain.Connection;
using UnityEngine;
using Master.Domain.GameEvents;

namespace Master.Domain.BehaviorTree
{
    public enum TreeType
    {
        Glycemia,
        Energy,
        Hunger
    }

    public abstract class BTree
    {
        private Node _root;
        private Queue<AttributeUpdateIntervalInfo> _intervalInfoQueue = new Queue<AttributeUpdateIntervalInfo>();

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

        public void EnqueueAttribute(AttributeUpdateIntervalInfo intervalInfo)
        {
            bool IsInTime = _settingsManager.IsInRange(intervalInfo.dateTime.TimeOfDay);
            bool IsInCurrentDate = intervalInfo.dateTime.Date == _connectionManager.currentConnectionDateTime.Date;
            bool IsInLastSessionDate = intervalInfo.dateTime.Date == _connectionManager.lastDisconnectionDateTime.Date;

            if (IsInLastSessionDate && IsInTime || IsInCurrentDate && IsInTime)
            {
                _intervalInfoQueue.Enqueue(intervalInfo);
            }
        }

        public void Run()
        {
            while (_intervalInfoQueue.Count > 0)
            {
                AttributeUpdateIntervalInfo currentIntervalInfo = _intervalInfoQueue.Peek();

                if (_intervalInfoQueue.Peek().dateTime.TimeOfDay == _settingsManager.initialTime)
                {
                    RestartAttribute(currentIntervalInfo.dateTime);
                }
                else
                {
                    StartStash();

                    NodeState result = _root.Evaluate(currentIntervalInfo);

                    while (result == NodeState.RUNNING)
                    {
                        result = _root.Evaluate(currentIntervalInfo);
                    }
                }

                ApplyStash(currentIntervalInfo.dateTime);
                GameEvents_PetCare.OnFinishedExecutionAttributesBTree?.Invoke();
                _intervalInfoQueue.Dequeue();
            }
        }

        protected abstract Node SetUpTree();

        private void RestartAttribute(DateTime time)
        {
            switch (_treeType)
            {
                case TreeType.Glycemia:
                    _petCareManager.RestartGlycemia(time);
                    break;
                case TreeType.Energy:
                    _petCareManager.RestartEnergy(time);
                    break;
                case TreeType.Hunger:
                    _petCareManager.RestartHunger(time);
                    break;
            }
        }

        private void StartStash()
        {
            switch (_treeType)
            {
                case TreeType.Glycemia:
                    _petCareManager.StartStashGlycemia();
                    break;
                case TreeType.Energy:
                    _petCareManager.StartStashEnergy();
                    break;
                case TreeType.Hunger:
                    _petCareManager.StartStashHunger();
                    break;
            }
        }

        private void ApplyStash(DateTime time)
        {
            switch (_treeType)
            {
                case TreeType.Glycemia:
                    _petCareManager.ApplyStashedGlycemia(time);
                    break;
                case TreeType.Energy:
                    _petCareManager.ApplyStashedEnergy(time);
                    break;
                case TreeType.Hunger:
                    _petCareManager.ApplyStashedHunger(time);
                    break;
            }
        }
    }
}
