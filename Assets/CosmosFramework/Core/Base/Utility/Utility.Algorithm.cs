using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public sealed partial class Utility
    {
        /// <summary>
        /// 通用算法工具类，封装了常用算法。
        /// </summary>
        public static class Algorithm
        {
            /// <summary>
            /// 升序排序
            /// </summary>
            public static void SortByAscending<T, K>(T[] array, Func<T, K> handler)
                where K : IComparable<K>
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (handler(array[i]).CompareTo(handler(array[j])) < 0)
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
            public static void SortByDescending<T, K>(T[] array, Func<T, K> handler)
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
            public static T Min<T, K>(T[] array, Func<T, K> handler)
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
            public static T Max<T, K>(T[] array, Func<T, K> handler)
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
            public static T Find<T>(T[] array, Predicate<T> handler)
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
            public static T[] FindAll<T, K>(T[] array, Predicate<T> handler)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (handler(array[i]))
                        list.Add(array[i]);
                }
                return list.ToArray();
            }
            /// <summary>
            /// 泛型二分查找，需要传入升序数组
            /// </summary>
            /// <returns>返回对象在数组中的序号，若不存在，则返回-1</returns>
            public static int BinarySearch<T, K>(T[] array, K target, Func<T, K> handler)
                where K : IComparable<K>
            {
                int first = 0;
                int last = array.Length - 1;
                while (first <= last)
                {
                    int mid = first + (last - first) / 2;
                    if (handler(array[mid]).CompareTo(target) > 0)
                        last = mid - 1;
                    else if (handler(array[mid]).CompareTo(target) < 0)
                        first = mid + 1;
                    else
                        return mid;
                }
                return -1;
            }
            /// <summary>
            /// 使用双精度数组随机生成不重复的ID
            /// </summary>
            /// <param name="length">数组长度</param>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int数组</returns>
            public static int[] DoubleArrayToNonRepeatedRandom(int length, int minValue, int maxValue)
            {
                int seed = Guid.NewGuid().GetHashCode();
                Random radom = new Random(seed);
                int[] index = new int[length];
                for (int i = 0; i < length; i++)
                {
                    index[i] = i + 1;
                }
                int[] array = new int[length]; // 用来保存随机生成的不重复的数 
                int site = length;             // 设置上限 
                int idx;                       // 获取index数组中索引为idx位置的数据，赋给结果数组array的j索引位置
                for (int j = 0; j < length; j++)
                {
                    idx = radom.Next(0, site - 1);  // 生成随机索引数
                    array[j] = index[idx];          // 在随机索引位置取出一个数，保存到结果数组 
                    index[idx] = index[site - 1];   // 作废当前索引位置数据，并用数组的最后一个数据代替之
                    site--;                         // 索引位置的上限减一（弃置最后一个数据）
                }
                return array;
            }
            /// <summary>
            /// 将一个int数组转换为顺序的整数;
            /// 若数组中存在负值，则默认将负值取绝对值
            /// </summary>
            /// <param name="intArray">传入的数组</param>
            /// <returns>转换成整数后的int</returns>

            public static int ConvertIntArrayToInt(int[] intArray)
            {
                int result = 0;
                int length = intArray.Length;
                for (int i = 0; i < length; i++)
                {
                    result += Convert.ToInt32((Math.Abs(intArray[i]) * Math.Pow(10, length - 1 - i)));
                }
                return result;
            }
            /// <summary>
            /// 生成指定长度的int整数
            /// </summary>
            /// <param name="length">数组长度</param>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int CreateRandomInt(int length, int minValue, int maxValue)
            {
                var arr = DoubleArrayToNonRepeatedRandom(length, minValue, maxValue);
                return ConvertIntArrayToInt(arr);
            }
            /// <summary>
            /// 随机在范围内生成一个int
            /// </summary>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int CreateRandomInt(int minValue, int maxValue)
            {
                int seed = Guid.NewGuid().GetHashCode();
                Random random = new Random(seed);
                int result = random.Next(minValue, maxValue);
                return result;
            }
            /// <summary>
            /// 交换两个值
            /// </summary>
            /// <typeparam name="T">传入的对象类型</typeparam>
            /// <param name="t1">第一个需要交换的值</param>
            /// <param name="t2">第二个需要交换的值</param>
            public static void Swap<T>(ref T t1, ref T t2)
            {
                T t3 = t1;
                t1 = t2;
                t2 = t3;
            }
        }
    }
}
