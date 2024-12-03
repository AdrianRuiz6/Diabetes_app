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
            Debug.Log($"[BTree] Awake: Registrando evento OnExecutingAttributes.");
            GameEventsPetCare.OnExecutingAttributes += ExecutingAttribute;
        }

        void OnDestroy()
        {
            Debug.Log($"[BTree] OnDestroy: Desregistrando evento OnExecutingAttributes.");
            GameEventsPetCare.OnExecutingAttributes -= ExecutingAttribute;
        }

        protected virtual void Start()
        {
            Debug.Log($"[BTree] Start: Inicializando �rbol de comportamiento.");
            _root = SetUpTree();
        }

        protected void SetState(TreeType newState)
        {
            treeType = newState;
            Debug.Log($"[BTree] SetState: Estado cambiado a {treeType}");
        }

        void Update()
        {
            Debug.Log($"[BTree] Update {treeType}: _dateTimesQueue.Count = {_dateTimesQueue.Count}");

            while (_dateTimesQueue.Count > 0)
            {
                DateTime nextDateTime = _dateTimesQueue.Peek();
                Debug.Log($"[BTree] Update {treeType}: Procesando siguiente DateTime = {nextDateTime}, TimeOfDay = {nextDateTime.TimeOfDay}");

                if (_dateTimesQueue.Peek().TimeOfDay == LimitHours.Instance.initialTime)
                {
                    Debug.Log($"[BTree] Update {treeType}: RestartAttributes llamado para {nextDateTime}");
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
                        Debug.LogError($"[BTree] Update: Excepci�n al llamar a RestartAttributes: {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else
                {
                    NodeState result = _root.Evaluate(nextDateTime);
                    Debug.Log($"[BTree] Update {treeType}: Evaluaci�n del nodo inicial, result = {result}");

                    while (result == NodeState.RUNNING)
                    {
                        result = _root.Evaluate(nextDateTime);
                        Debug.Log($"[BTree] Update {treeType}: Reevaluando nodo en ejecuci�n, result = {result}");
                    }
                }

                _dateTimesQueue.Dequeue();
                Debug.Log($"[BTree] Update {treeType}: Elemento procesado y eliminado de la cola. _dateTimesQueue.Count despu�s del Dequeue = {_dateTimesQueue.Count}");
            }

            if (_dateTimesQueue.Count == 0)
            {
                Debug.Log($"[BTree] Update {treeType}: La cola est� vac�a. Terminando la ejecuci�n del while.");
            }
        }

        private void ExecutingAttribute(DateTime currentDateTime)
        {
            Debug.Log($"[BTree] ExecutingAttribute: Nueva ejecuci�n con DateTime = {currentDateTime}");
            Debug.Log($"[BTree] ExecutingAttribute: initialTime = {LimitHours.Instance.initialTime}, finishTime = {LimitHours.Instance.finishTime}");

            if (firstIteration)
            {
                TimeSpan previousTime = currentDateTime.AddSeconds(-AttributeSchedule.Instance.UpdateInterval).TimeOfDay;
                previousIsInTime = LimitHours.Instance.IsInRange(previousTime);
                firstIteration = false;
                Debug.Log($"[BTree] ExecutingAttribute: Primer inicio. previousIsInTime = {previousIsInTime}, previousTime = {previousTime}");
            }

            bool currentIsInTime = LimitHours.Instance.IsInRange(currentDateTime.TimeOfDay);
            Debug.Log($"[BTree] ExecutingAttribute: currentIsInTime = {currentIsInTime}, previousIsInTime = {previousIsInTime}");

            if (currentIsInTime)
            {
                Debug.Log($"[BTree] ExecutingAttribute: Dentro de la franja horaria.");

                if (previousIsInTime == false && currentDateTime.TimeOfDay != LimitHours.Instance.initialTime)
                {
                    previousIsInTime = currentIsInTime;
                    Debug.Log($"[BTree] ExecutingAttribute: Entrada en la franja horaria. Calculando initialTime.");
                    DateTime initialTime = currentDateTime.Date.AddHours(LimitHours.Instance.initialTime.Hours);
                    AddToEvalueateQueue(initialTime);
                }

                Debug.Log($"[BTree] ExecutingAttribute: A�adiendo {currentDateTime} a la cola.");
                AddToEvalueateQueue(currentDateTime);
            }
            else
            {
                Debug.Log($"[BTree] ExecutingAttribute: Fuera de la franja horaria.");
                if (previousIsInTime == true)
                {
                    previousIsInTime = currentIsInTime;
                    Debug.Log($"[BTree] ExecutingAttribute: Salida de la franja horaria. Calculando finishTime.");
                    DateTime finishTime = currentDateTime.Date.AddHours(LimitHours.Instance.finishTime.Hours)
                        .AddMinutes(LimitHours.Instance.finishTime.Minutes);
                    AddToEvalueateQueue(finishTime);
                }
            }
            Debug.Log($"[BTree] ExecutingAttribute: Actualizaci�n completada. previousIsInTime = {previousIsInTime}");
        }

        private void AddToEvalueateQueue(DateTime newDateTime)
        {
            Debug.Log($"[BTree] AddToEvalueateQueue: Intentando a�adir {newDateTime} a la cola.");

            _dateTimesQueue.Enqueue(newDateTime);
            Debug.Log($"[BTree] AddToEvalueateQueue: A�adido. _dateTimesQueue.Count = {_dateTimesQueue.Count}");
        }

        protected abstract Node SetUpTree();
    }
}
