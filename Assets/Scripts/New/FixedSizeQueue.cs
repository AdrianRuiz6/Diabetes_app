using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSizeQueue<T> : IEnumerable<T>
{
    private readonly int maxSize = QuestionsManager.Instance.maxQuestionIndex;
    private readonly Queue<T> queue;

    public FixedSizeQueue()
    {
        this.queue = new Queue<T>();
    }

    public void Enqueue(T item)
    {
        if (queue.Count == maxSize)
        {
            queue.Dequeue();
        }
        queue.Enqueue(item);
    }

    public void Clear()
    {
        queue.Clear();
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
