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
        private int _counterEvaluates = 0;

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
                NodeState result = _root.Evaluate();
                while (result == NodeState.RUNNING)
                {
                    result = _root.Evaluate();
                }
                _counterEvaluates -= 1;
            }
        }

        private void ExecutingAttribute()
        {
            _counterEvaluates += 1;
        }

        protected abstract Node SetUpTree();
    }
}



