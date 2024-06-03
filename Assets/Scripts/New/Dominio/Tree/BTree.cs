using Master.Domain.Events;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node _root = null;

        void Awake()
        {
            GameEventsPetCare.OnExecutingAttributes += Evaluate;
        }

        void OnDestroy()
        {
            GameEventsPetCare.OnExecutingAttributes -= Evaluate;
        }

        protected virtual void Start()
        {
            _root = SetUpTree();
        }

        private void Evaluate()
        {
            AttributesMutex.Instance.WaitForMutex();
            AttributesMutex.Instance.Lock();
            _root.Evaluate();
            AttributesMutex.Instance.Unlock();
        }

        protected abstract Node SetUpTree();
    }
}



