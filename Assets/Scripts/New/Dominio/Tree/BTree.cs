using Master.Domain.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node _root = null;

        private bool previousIsInTime = true;
        private bool firstIteration = true;

        private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

        public enum TreeType
        {
            Glycemia,
            Activity,
            Hunger
        }
        protected TreeType treeType;

        void Awake()
        {
            GameEvents_PetCare.OnExecutingAttributes += EnqueueAttribute;
        }

        void OnDestroy()
        {
            GameEvents_PetCare.OnExecutingAttributes -= EnqueueAttribute;
        }

        protected virtual void Start()
        {
            _root = SetUpTree();
        }

        protected void SetState(TreeType newState)
        {
            treeType = newState;
        }

        void Update()
        {
            while (_dateTimesQueue.Count > 0)
            {
                DateTime nextDateTime = _dateTimesQueue.Peek();

                if (_dateTimesQueue.Peek().TimeOfDay == LimitHours.Instance.initialTime)
                {
                    try
                    {
                        switch (treeType)
                        {
                            case TreeType.Glycemia:
                                AttributeManager.Instance.RestartGlycemia(nextDateTime);
                                break;
                            case TreeType.Activity:
                                AttributeManager.Instance.RestartActivity(nextDateTime);
                                break;
                            case TreeType.Hunger:
                                AttributeManager.Instance.RestartHunger(nextDateTime);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[BTree] Update: Excepción al llamar a RestartAttributes: {ex.Message}\n{ex.StackTrace}");
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

        private void EnqueueAttribute(DateTime currentDateTime)
        {
            bool IsInTime = LimitHours.Instance.IsInRange(currentDateTime.TimeOfDay);
            bool IsInCurrentDate = currentDateTime.Date == TimeManager.Instance.currentConnectionDateTime.Date;
            bool IsInLastSessionDate = currentDateTime.Date == TimeManager.Instance.lastDisconnectionDateTime.Date;

            if (IsInLastSessionDate && IsInTime || IsInCurrentDate && IsInTime)
            {
                _dateTimesQueue.Enqueue(currentDateTime);
            }
        }

        protected abstract Node SetUpTree();
    }
}
