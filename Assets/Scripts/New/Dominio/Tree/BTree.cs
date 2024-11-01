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
        private int _counterEvaluates = 0;

        private bool previousIsInTime = true;

        private Queue<DateTime> _dateTimesQueue = new Queue<DateTime>();

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

        void Update()
        {
            while (_counterEvaluates > 0)
            {
                if(_dateTimesQueue.Peek().TimeOfDay == LimitHours.Instance.initialTime)
                {
                    AttributeManager.Instance.RestartAttributes(_dateTimesQueue.Peek());
                }
                else
                {
                    NodeState result = _root.Evaluate(_dateTimesQueue.Peek());
                    while (result == NodeState.RUNNING)
                    {
                        result = _root.Evaluate(_dateTimesQueue.Peek());
                    }
                }
                _dateTimesQueue.Dequeue();
                _counterEvaluates -= 1;
            }
        }

        private void ExecutingAttribute(DateTime currentDateTime) // TODO: modificar Debugs.
        {
            DateTime previousDateTime = currentDateTime.AddSeconds(-AttributeSchedule.Instance.UpdateInterval);
            previousIsInTime = previousDateTime.TimeOfDay >= LimitHours.Instance.initialTime && previousDateTime.TimeOfDay <= LimitHours.Instance.finishTime;

            Debug.Log($"FRANJA - Nueva ejecución -----------");
            // Si está en la franja horaria en los que se mueven los atributos.
            if (currentDateTime.TimeOfDay >= LimitHours.Instance.initialTime && currentDateTime.TimeOfDay <= LimitHours.Instance.finishTime)
            {
                Debug.Log($"FRANJA - Está DENTRO de la franja horaria");
                // Si acaba de entrar en la franja horaria calcular el initialTime también.
                if(previousIsInTime == false && currentDateTime.TimeOfDay != LimitHours.Instance.initialTime)
                {
                    Debug.Log($"FRANJA - Se acaba de ENTRAR en la franja horaria.");
                    DateTime initialTime = currentDateTime.Date.AddHours(LimitHours.Instance.initialTime.Hours);
                    AddToEvalueateQueue(initialTime);
                }
                Debug.Log($"FRANJA - Hora: {currentDateTime}");
                AddToEvalueateQueue(currentDateTime);

                
                if (currentDateTime.TimeOfDay == LimitHours.Instance.finishTime)
                {
                    previousIsInTime = false;
                }
                else
                {
                    previousIsInTime = true;
                }
            }
            else // Si acaba de salir de la franja horaria se calcula el finishTime.
            {
                Debug.Log($"FRANJA - Está FUERA en la franja horaria");
                if (previousIsInTime == true)
                {
                    Debug.Log($"FRANJA - Se acaba de SALIR de la franja horaria.");
                    DateTime finishTime = currentDateTime.Date.AddHours(LimitHours.Instance.finishTime.Hours);
                    finishTime = currentDateTime.Date.AddMinutes(LimitHours.Instance.finishTime.Minutes);
                    AddToEvalueateQueue(finishTime);
                }

                previousIsInTime = false;
            }
            
        }

        private void AddToEvalueateQueue(DateTime newDateTime)
        {
            _dateTimesQueue.Enqueue(newDateTime);
            _counterEvaluates += 1;
        }

        protected abstract Node SetUpTree();
    }
}



