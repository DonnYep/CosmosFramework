﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos
{
    public static class IEnumerableExts
    {
        private static readonly Random _rnd = new Random();
        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] enumerables)
        {
            return enumerables.SelectMany(e => e);
        }
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> @this, params T[] values)
        {
            foreach (T item in @this)
                yield return item;
            foreach (T value in values)
                yield return value;
        }
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> @this, T value)
        {
            foreach (T item in @this)
                yield return item;
            yield return value;
        }
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> @this)
        {
            return @this.SelectMany(i => i);
        }
        public static IEnumerable<T> Flatten<TKey, T>(this IEnumerable<IDictionary<TKey, T>> @this)
        {
            return @this.SelectMany(i => i.Values);
        }
        public static IEnumerable<T> Flatten<TKey, T>(this IDictionary<TKey, IEnumerable<T>> @this)
        {
            return @this.Values.SelectMany(i => i);
        }
        public static IEnumerable<T> Flatten<TKey1, TKey2, T>(this IDictionary<TKey1, IDictionary<TKey2, T>> @this)
        {
            return @this.Values.SelectMany(i => i.Values);
        }
        public static int IndexOf<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;
            int i = 0;
            foreach (T item in @this)
            {
                if (comparer.Equals(item, value))
                    return i;
                i++;
            }
            return -1;
        }
        public static int IndexOf<T>(this IEnumerable<T> @this, Func<T, bool> condition)
        {
            int i = 0;
            foreach (T item in @this)
            {
                if (condition(item))
                    return i;
                i++;
            }
            return -1;
        }
        public static int LastIndexOf<T>(this IEnumerable<T> @this, T value, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<T>.Default;
            int i = 0, index = -1;
            foreach (T item in @this)
            {
                if (comparer.Equals(item, value))
                    index = i;
                i++;
            }
            return index;
        }
        public static int LastIndexOf<T>(this IEnumerable<T> @this, Func<T, bool> condition)
        {
            int i = 0, index = -1;
            foreach (T item in @this)
            {
                if (condition(item))
                    index = i;
                i++;
            }
            return index;
        }
        public static IEnumerable<T> Inverse<T>(this IEnumerable<T> @this)
        {
            var list = @this as IList<T>;
            if (list != null)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                    yield return list[i];
            }
            else
            {
                foreach (T item in @this.Reverse())
                    yield return item;
            }
        }
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> @this, int size)
        {
            T[] bucket = null;
            int count = 0;
            foreach (T item in @this)
            {
                if (bucket == null)
                    bucket = new T[size];
                bucket[count++] = item;
                if (count != size)
                    continue;
                yield return bucket;
                bucket = null;
                count = 0;
            }
            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> @this, Random rnd = null)
        {
            return @this.OrderBy(i => (rnd ?? _rnd).NextDouble());
        }
        public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(this IEnumerable<T1> source, IEnumerable<T2> second, IEnumerable<T3> third, Func<T1, T2, T3, TResult> selector)
        {
            using (var e1 = source.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            using (var e3 = third.GetEnumerator())
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                    yield return selector(e1.Current, e2.Current, e3.Current);
        }
        public static string ConcatString<T>(this IEnumerable<T> @this)
        {
            return string.Concat(@this);
        }
        public static string ConcatString(this IEnumerable<string> @this)
        {
            return string.Concat(@this);
        }

        public static IList<T> AsList<T>(this IEnumerable<T> @this)
        {
            return @this as IList<T> ?? @this.ToList();
        }

        public static Dictionary<TKey, T> ToDictionary<TKey, T>(this IEnumerable<KeyValuePair<TKey, T>> @this)
        {
            return @this.ToDictionary(p => p.Key, p => p.Value);
        }
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        {
            return new HashSet<T>(@this);
        }
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(@this, comparer);
        }
        public static HashSet<TKey> ToHashSet<TKey, T>(this IEnumerable<T> @this, Func<T, TKey> keySelector)
        {
            return new HashSet<TKey>(@this.Select(keySelector));
        }
        public static HashSet<TKey> ToHashSet<TKey, T>(this IEnumerable<T> @this, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new HashSet<TKey>(@this.Select(keySelector), comparer);
        }
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> @this)
        {
            return new SortedSet<T>(@this);
        }
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> @this, IComparer<T> comparer)
        {
            return new SortedSet<T>(@this, comparer);
        }
        public static SortedSet<TKey> ToSortedSet<TKey, T>(this IEnumerable<T> @this, Func<T, TKey> keySelector)
        {
            return new SortedSet<TKey>(@this.Select(keySelector));
        }
        public static SortedSet<TKey> ToSortedSet<TKey, T>(this IEnumerable<T> @this, Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new SortedSet<TKey>(@this.Select(keySelector), comparer);
        }
    }
}
