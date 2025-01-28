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
            GameEventsPetCare.OnExecutingAttributes += ExecutingAttribute;
        }

        void OnDestroy()
        {
            GameEventsPetCare.OnExecutingAttributes -= ExecutingAttribute;
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

        private void ExecutingAttribute(DateTime currentDateTime)
        {
            if (firstIteration)
            {
                TimeSpan previousTime = currentDateTime.AddSeconds(-AttributeSchedule.Instance.UpdateInterval).TimeOfDay;
                previousIsInTime = LimitHours.Instance.IsInRange(previousTime);
            }

            bool currentIsInTime = LimitHours.Instance.IsInRange(currentDateTime.TimeOfDay);

            if (currentIsInTime)
            {
                if (previousIsInTime == false && currentDateTime.TimeOfDay != LimitHours.Instance.initialTime)
                {
                    previousIsInTime = currentIsInTime;
                    DateTime initialTime = currentDateTime.Date.AddHours(LimitHours.Instance.initialTime.Hours);
                    AddToEvalueateQueue(initialTime);
                }

                AddToEvalueateQueue(currentDateTime);
            }
            else
            {
                if (previousIsInTime == true)
                {
                    previousIsInTime = currentIsInTime;
                    DateTime finishTime = currentDateTime.Date.AddHours(LimitHours.Instance.finishTime.Hours)
                        .AddMinutes(LimitHours.Instance.finishTime.Minutes);
                    AddToEvalueateQueue(finishTime);
                }
            }
            firstIteration = false;
        }

        private void AddToEvalueateQueue(DateTime newDateTime)
        {
            _dateTimesQueue.Enqueue(newDateTime);
        }

        protected abstract Node SetUpTree();
    }
}
