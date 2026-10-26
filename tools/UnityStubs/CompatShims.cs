// Compile-check compatibility shims for framework code targeting newer BCL APIs.
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static class HashSetCompatExtensions
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            if (set == null || items == null)
                return;
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}
