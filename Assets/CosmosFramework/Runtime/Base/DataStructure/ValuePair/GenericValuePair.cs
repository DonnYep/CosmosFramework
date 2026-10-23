using System;
using System.Collections.Generic;

namespace Cosmos
{
    /// <summary>
    /// 双泛型值组合；此组合并非Key-Value形式，而是Value-Value形式。
    /// 比较时调用Equals方法；进行组合时注意覆写Equals；
    /// </summary>
    /// <typeparam name="TValue1">泛型类型</typeparam>
    /// <typeparam name="TValue2">泛型类型</typeparam>
    public struct GenericValuePair<TValue1, TValue2> : IEquatable<GenericValuePair<TValue1, TValue2>>
    {
        TValue1 value1;
        TValue2 value2;
        public TValue1 Value1 { get { return value1; } }
        public TValue2 Value2 { get { return value2; } }

        public GenericValuePair(TValue1 value1, TValue2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }
        public static bool operator ==(GenericValuePair<TValue1, TValue2> lhs, GenericValuePair<TValue1, TValue2> rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(GenericValuePair<TValue1, TValue2> lhs, GenericValuePair<TValue1, TValue2> rhs)
        {
            return !lhs.Equals(rhs);
        }
        /// <summary>
        /// Equals方法需要注意，若在对象未覆写Equals时，值类型比较值是否相同，引用类型比较地址是否相同；
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is GenericValuePair<TValue1, TValue2> && Equals((GenericValuePair<TValue1, TValue2>)obj);
        }
        public override int GetHashCode()
        {
            //使用默认比较器，兼容 null 字段与默认结构体
            unchecked
            {
                return (EqualityComparer<TValue1>.Default.GetHashCode(value1) * 397)
                    ^ EqualityComparer<TValue2>.Default.GetHashCode(value2);
            }
        }
        public override string ToString()
        {
            if (Value1 == null)
                throw new ArgumentNullException($"GenericValuePair: {typeof(TValue1)} is  invalid");
            if (Value2 == null)
                throw new ArgumentNullException($"GenericValuePair: {typeof(TValue2)} is  invalid");
            return $"{typeof(TValue1)}：{Value1}；{typeof(TValue2)}：{Value2}";
        }
        /// <summary>
        /// Equals方法需要注意，若在对象未覆写Equals时，值类型比较值是否相同，引用类型比较地址是否相同；
        /// </summary>
        public bool Equals(GenericValuePair<TValue1, TValue2> other)
        {
            //使用默认比较器，避免 null 字段直接调用实例方法
            return EqualityComparer<TValue1>.Default.Equals(value1, other.value1)
                && EqualityComparer<TValue2>.Default.Equals(value2, other.value2);
        }
    }
}
