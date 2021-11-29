using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    public static class ListExts
    {
        private static readonly Random _rnd = new Random();
        public static TValue RemoveLast<TValue>(this List<TValue> @this)
        {
            var index = @this.Count - 1;
            var result = @this[index];
            @this.RemoveAt(index);
            return result;
        }
        public static TValue RemoveFirst<TValue>(this List<TValue> @this)
        {
            var result = @this[0];
            @this.RemoveAt(0);
            return result;
        }
        public static TValue First<TValue>(this List<TValue> @this)
        {
            var result = @this[0];
            return result;
        }
        public static TValue Last<TValue>(this List<TValue> @this)
        {
            var result = @this[@this.Count - 1];
            return result;
        }
        public static T RandomItem<T>(this IList<T> @this, Random rnd = null)
        {
            return @this[(rnd ?? _rnd).Next(@this.Count)];
        }
        public static void InsertRange<T>(this IList<T> @this, int index, IEnumerable<T> items)
        {
            foreach (T item in items)
                @this.Insert(index++, item);
        }
        public static T AtWrapped<T>(this IList<T> @this, int index)
        {
            return @this[WrapIndex(index, @this.Count)];
        }
        public static T AtWrappedOrDefault<T>(this IList<T> @this, int index, T defaultValue = default(T))
        {
            return @this.Count > 0 ? @this[WrapIndex(index, @this.Count)] : defaultValue;
        }
        public static void SetAtWrapped<T>(this IList<T> @this, int index, T value)
        {
            @this[WrapIndex(index, @this.Count)] = value;
        }
        public static int IndexOfOrDefault<T>(this IList<T> @this, T value, int defaultIndex)
        {
            int index = @this.IndexOf(value);
            return index != -1 ? index : defaultIndex;
        }
        public static void ClearAndDispose<T>(this IList<T> @this) where T : IDisposable
        {
            foreach (T item in @this)
                item.Dispose();
            @this.Clear();
        }
        public static void AddRangeUntyped(this IList @this, IEnumerable items)
        {
            foreach (object item in items)
                @this.Add(item);
        }
        public static void RemoveRangeUntyped(this IList @this, IEnumerable items)
        {
            foreach (object item in items)
                @this.Remove(item);
        }
        public static void ReplaceUntyped(this IList @this, IEnumerable items)
        {
            @this.Clear();
            @this.AddRangeUntyped(items);
        }
        private static int WrapIndex(int index, int count)
        {
            if (count == 0)
                throw new IndexOutOfRangeException();
            else if (index >= count)
                index = index % count;
            else if (index < 0)
            {
                index = index % count;
                if (index != 0)
                    index += count;
            }
            return index;
        }
    }
}
