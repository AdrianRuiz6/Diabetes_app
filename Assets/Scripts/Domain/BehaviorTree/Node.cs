using System;
using System.Collections;
using System.Collections.Generic;

namespace Master.Domain.BehaviorTree

{
    public enum NodeState
    {
        RUNNING, SUCCESS, FAILURE
    }

    public class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children;

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> mychildren)
        {
            children = new List<Node>();
            foreach (Node child in mychildren)
            {
                child.parent = this;
                children.Add(child);
            }
        }

        public virtual NodeState Evaluate(DateTime currentDateTime) => NodeState.FAILURE;



        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public void setData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key) {
            object value = null;

            if (_dataContext.TryGetValue(key, out value))
                return value;
            else
            {
                Node node = parent;
                while (node != null)
                {
                    value = node.GetData(key);
                    if (value != null)
                        return value;

                    node = node.parent;
                }
                return null;
            }
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsValue(key))
            {
                _dataContext.Remove(key);
                return true;
            }
            else
            {
                Node node = parent;
                while (node != null)
                {
                    bool cleared = node.ClearData(key);
                    if (cleared)
                        return true;

                    node = node.parent;
                }
                return false;
            }
        }
    } 
} 
