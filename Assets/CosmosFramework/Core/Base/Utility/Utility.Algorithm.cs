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
            static Random random = new Random(Guid.NewGuid().GetHashCode());
            /// <summary>
            /// 快速排序：降序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            /// <param name="start">起始位</param>
            /// <param name="end">结束位</param>
            public static void SortByDescend<T, K>(T[] array, Func<T, K> handler, int start, int end)
                where K : IComparable<K>
            {
                if (array == null)
                    throw new ArgumentNullException("SortByDescend : array is null");
                if (handler == null)
                    throw new ArgumentNullException("SortByDescend : handler is null");
                if (start < 0 || end < 0 || start >= end)
                {
                    return;
                }
                int pivort = start;
                T pivortValue = array[pivort];
                Swap(array, end, pivort);
                int storeIndex = start;
                for (int i = start; i <= end - 1; i++)
                {
                    if (handler(array[i]).CompareTo(handler(pivortValue)) > 0)
                    {
                        Swap(array, i, storeIndex);
                        storeIndex++;
                    }
                }
                Swap(array, storeIndex, end);
                SortByDescend(array, handler, start, storeIndex - 1);
                SortByDescend(array, handler, storeIndex + 1, end);
            }
            /// <summary>
            /// 快速排序：升序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            /// <param name="start">起始位</param>
            /// <param name="end">结束位</param>
            public static void SortByAscend<T, K>(T[] array, Func<T, K> handler, int start, int end)
                where K : IComparable<K>
            {
                if (array == null)
                    throw new ArgumentNullException("QuickSortByAscend : array is null");
                if (handler == null)
                    throw new ArgumentNullException("QuickSortByAscend : handler is null");
                if (start < 0 || end < 0 || start >= end)
                {
                    return;
                }
                int pivort = start;
                T pivortValue = array[pivort];
                Swap(array, end, pivort);
                int storeIndex = start;
                for (int i = start; i <= end - 1; i++)
                {
                    if (handler(array[i]).CompareTo(handler(pivortValue)) < 0)
                    {
                        Swap(array, i, storeIndex);
                        storeIndex++;
                    }
                }
                Swap(array, storeIndex, end);
                SortByAscend(array, handler, start, storeIndex - 1);
                SortByAscend(array, handler, storeIndex + 1, end);
            }
            /// <summary>
            /// 冒泡排序：升序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            public static void SortByAscend<T, K>(T[] array, Func<T, K> handler)
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
            /// 冒泡排序：降序
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <typeparam name="K">比较类型</typeparam>
            /// <param name="array">需要排序的数组对象</param>
            /// <param name="handler">排序条件</param>
            public static void SortByDescend<T, K>(T[] array, Func<T, K> handler)
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
            public static T[] FindAll<T>(T[] array, Predicate<T> handler)
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
            /// <param name="length">数值长度</param>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int RandomRange(int length, int minValue, int maxValue)
            {
                if (minValue >= maxValue)
                    throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
                string buffer = "0123456789";// 随机字符中也可以为汉字（任何）
                StringBuilder strbuilder = new StringBuilder();
                int range = buffer.Length;
                int resultValue = 0;
                do
                {
                    for (int i = 0; i < length; i++)
                    {
                        strbuilder.Append(buffer.Substring(random.Next(range), 1));
                    }
                    resultValue = Int32.Parse(strbuilder.ToString());
                } while (resultValue > maxValue || resultValue < minValue);
                return resultValue;
            }
            /// <summary>
            /// 随机在范围内生成一个int
            /// </summary>
            /// <param name="minValue">随机取值最小区间</param>
            /// <param name="maxValue">随机取值最大区间</param>
            /// <returns>生成的int整数</returns>
            public static int RandomRange(int minValue, int maxValue)
            {
                if (minValue >= maxValue)
                    throw new ArgumentNullException("RandomRange : minValue is greater than or equal to maxValue");
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
            /// <summary>
            /// 交换数组中的两个元素
            /// </summary>
            /// <typeparam name="T">传入的对象类型</typeparam>
            /// <param name="array">传入的数组</param>
            /// <param name="idxA">序号A</param>
            /// <param name="idxB">序号B</param>
            public static void Swap<T>(T[] array, int idxA, int idxB)
            {
                T temp = array[idxA];
                array[idxA] = array[idxB];
                array[idxB] = temp;
            }
            /// <summary>
            /// 随机打乱数组
            /// </summary>
            /// <typeparam name="T">数组类型</typeparam>
            /// <param name="array">数组</param>
            public static void Disrupt<T>(T[] array)
            {
                int index = 0;
                T tmp;
                for (int i = 0; i < array.Length; i++)
                {
                    index = RandomRange(0, array.Length);
                    if (index != i)
                    {
                        tmp = array[i];
                        array[i] = array[index];
                        array[index] = tmp;
                    }
                }
            }
            /// <summary>
            /// 产生均匀随机数
            /// </summary>
            public static double AverageRandom(double minValue, double maxValue)
            {
                int min = (int)(minValue * 10000);
                int max = (int)(maxValue * 10000);
                int result = random.Next(min, max);
                return result / 10000.0;
            }
            /// <summary>
            /// 正态分布概率密度函数
            /// </summary>
            public static double NormalDistributionProbability(double x, double miu, double sigma)
            {
                return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
            }
            /// <summary>
            /// 随机正态分布；
            /// </summary>
            public static double RandomNormalDistribution(double miu, double sigma, double min, double max)//产生正态分布随机数
            {
                double x;
                double dScope;
                double y;
                do
                {
                    x = AverageRandom(min, max);
                    y = NormalDistributionProbability(x, miu, sigma);
                    dScope = AverageRandom(0, NormalDistributionProbability(miu, miu, sigma));
                } while (dScope > y);
                return x;
            }
            public static bool IsAdd(long value)
            {
                return !Convert.ToBoolean(value & 0x1);
            }
        }
    }
}
