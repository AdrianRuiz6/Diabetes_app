using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions
{
    public static void CopyDictionaryPerformance<TKey, TValue>(Dictionary<TKey, FixedSizeQueue<TValue>> source, Dictionary<TKey, FixedSizeQueue<TValue>> destination)
    {
        destination.Clear();
        foreach (var kvp in source)
        {
            var value = kvp.Value;
            var newQueue = new FixedSizeQueue<TValue>();

            foreach (var item in value)
            {
                newQueue.Enqueue(item);
            }

            destination[kvp.Key] = newQueue;
        }
    }

    public static void CopyFixedQueue<T>(FixedSizeQueue<T> source, FixedSizeQueue<T> destination)
    {
        destination.Clear();

        foreach (var item in source)
        {
            destination.Enqueue(item);
        }
    }

    public static void CopyList<T>(List<T> source, List<T> destination)
    {
        destination.Clear();

        foreach (var item in source)
        {
            destination.Add(item);
        }
    }
}
