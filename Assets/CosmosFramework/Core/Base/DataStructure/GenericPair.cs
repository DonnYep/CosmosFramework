using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public struct GenericPair<TValue1, TValue2> : IEquatable<GenericPair<TValue1, TValue2>>
    {
        TValue1 value1;
        TValue2 value2;
        public TValue1 Value1 { get { return value1; } }
        public TValue2 Value2 { get { return value2; } }

        public GenericPair(TValue1 value1, TValue2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }
        public void SetValue(TValue1 value1, TValue2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }
        public static bool operator ==(GenericPair<TValue1, TValue2> a, GenericPair<TValue1, TValue2> b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(GenericPair<TValue1, TValue2> a, GenericPair<TValue1, TValue2> b)
        {
            return !a.Equals(b);
        }
        /// <summary>
        /// Equals方法需要注意，若在对象未覆写Equals时，值类型比较值是否相同，引用类型比较地址是否相同；
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is GenericPair<TValue1, TValue2> && Equals((GenericPair<TValue1, TValue2>)obj);
        }
        public override int GetHashCode()
        {
            return value1.GetHashCode() ^ value2.GetHashCode();
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
        public bool Equals(GenericPair<TValue1, TValue2> other)
        {
            bool result = false;
            if (this.GetType() == other.GetType())
            {
                result = this.value1.Equals(other.value1) && this.value2.Equals(other.value2);
            }
            return result;
        }
    }
}
