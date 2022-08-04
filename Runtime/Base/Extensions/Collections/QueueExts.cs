using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cosmos
{
    public static class QueueExts
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (T item in items)
                queue.Enqueue(item);
        }
        public static void Clear<TValue>(this ConcurrentQueue<TValue> @this)
        {
            while (@this.Count > 0)
            {
                @this.TryDequeue(out _);
            }
        }
    }
}
