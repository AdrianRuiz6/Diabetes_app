using System.Collections.Generic;
namespace Master.Auxiliar
{
    public static class UtilityFunctions
    {
        private static System.Random random = new System.Random();

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

        public static void RandomizeList<T>(List<T> listToRandomize)
        {
            for (int i = listToRandomize.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = listToRandomize[i];
                listToRandomize[i] = listToRandomize[j];
                listToRandomize[j] = temp;
            }
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}