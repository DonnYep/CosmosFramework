using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
   public static partial class Utility
    {
        /// <summary>
        /// 通用算法工具类，封装了常用算法。
        /// </summary>
        public static class Algorithm
        {
            /// <summary>
            /// 升序排序
            /// </summary>
            public static void SortByOrder<T, K>(T[] array, CFResultAction<T, K> handler)
                where K : IComparable<K>
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                        {
                            T temp = array[i];
                            array[i] = array[j];
                            array[j] = temp;
                        }
                    }
                }
            }
            /// <summary>
            /// 降序排序
            /// </summary>
            public static void SortByDescending<T, K>(T[] array, CFResultAction<T, K> handler)
                where K : IComparable<K>
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (handler(array[i]).CompareTo(handler(array[j])) > 0)
                        {
                            T temp = array[i];
                            array[i] = array[j];
                            array[j] = temp;
                        }
                    }
                }
            }
            /// <summary>
            ///  获取最小
            /// </summary>
            public static T Min<T, K>(T[] array, CFResultAction<T, K> handler)
            where K : IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) > 0)
                    {
                        temp = arr;
                    }
                }
                return temp;
            }
            /// <summary>
            /// 获取最大值
            /// </summary>
            public static T Max<T, K>(T[] array, CFResultAction<T, K> handler)
            where K : IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (var arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) < 0)
                    {
                        temp = arr;
                    }
                }
                return temp;
            }
            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            public static T Find<T>(T[] array, CFPredicateAction<T> handler)
            {
                T temp = default(T);
                for (int i = 0; i < array.Length; i++)
                {
                    if (handler(array[i]))
                    {
                        return array[i];
                    }
                }
                return temp;
            }
            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            public static T[] FindAll<T, K>(T[] array, CFPredicateAction<T> handler)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (handler(array[i]))
                        list.Add(array[i]);
                }
                return list.ToArray();
            }
        }
    }
}
