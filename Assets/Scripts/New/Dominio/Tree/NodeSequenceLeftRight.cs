using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class NodeSequenceLeftRight : Node   //Nodo secuencia ejecución paralela;
    {
        private int _index;
        public NodeSequenceLeftRight() : base() {
            _index = -1; // El nodo no tiene hijos
        }
        public NodeSequenceLeftRight(List<Node> children) : base(children) {
            if (children != null)
                _index = 0;
            else
                _index = -1;
        }

        public override NodeState Evaluate()
        {
            switch (children[_index].Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    case NodeState.SUCCESS:
                        _index = (_index + 1);
                        if (_index != children.Count)
                        {
                            state = NodeState.RUNNING;
                            return state;
                        }
                        else
                        {
                            state = NodeState.SUCCESS;
                            _index = 0;
                            return state;
                        }
                }
            state = NodeState.SUCCESS;
            return state;
        }
    }
}

