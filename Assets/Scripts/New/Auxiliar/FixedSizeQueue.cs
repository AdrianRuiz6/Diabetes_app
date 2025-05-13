using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Master.Auxiliar
{
    public class FixedSizeQueue<T> : IEnumerable<T>
    {
        private readonly int maxSize = QuestionManager.Instance.maxQuestionIndex;
        private readonly Queue<T> queue;

        public FixedSizeQueue()
        {
            this.queue = new Queue<T>();
        }

        public FixedSizeQueue(List<T> list)
        {
            this.queue = new Queue<T>();
            if (list != null)
            {
                foreach (T element in list)
                {
                    queue.Enqueue(element);
                }
            }
        }

        public void Enqueue(T item)
        {
            if (queue.Count == maxSize)
            {
                queue.Dequeue();
            }
            queue.Enqueue(item);
        }

        public bool Contains(T item)
        {
            return queue.Contains(item);
        }

        public void Clear()
        {
            queue.Clear();
        }

        public int Count()
        {
            return queue.Count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}