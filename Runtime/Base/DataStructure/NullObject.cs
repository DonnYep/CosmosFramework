using System;
using System.ComponentModel;

namespace Cosmos
{
    /// <summary>
    /// 可空对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public readonly struct NullObject<T> : IComparable, IComparable<T>
    {
        [DefaultValue(true)]
        readonly bool isNull;
        public T Item { get; }
        private NullObject(T item, bool isnull) : this()
        {
            isNull = isnull;
            Item = item;
        }
        public NullObject(T item) : this(item, item == null) { }
        public static NullObject<T> Null()
        {
            return new NullObject<T>();
        }
        /// <summary>
        /// 是否是null
        /// </summary>
        /// <returns>是否为空</returns>
        public bool IsNull()
        {
            return isNull;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="nullObject">类型元素</param>
        public static implicit operator T(NullObject<T> nullObject)
        {
            return nullObject.Item;
        }
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="item">类型元素</param>
        public static implicit operator NullObject<T>(T item)
        {
            return new NullObject<T>(item);
        }
        public override string ToString()
        {
            return (Item != null) ? Item.ToString() : "NULL";
        }
        public int CompareTo(object value)
        {
            if (value is NullObject<T> nullObject)
            {
                if (nullObject.Item is IComparable c)
                {
                    return ((IComparable)Item).CompareTo(c);
                }
                return Item.ToString().CompareTo(nullObject.Item.ToString());
            }
            return 1;
        }
        public int CompareTo(T other)
        {
            if (other is IComparable c)
            {
                return ((IComparable)Item).CompareTo(c);
            }
            return Item.ToString().CompareTo(other.ToString());
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return IsNull();
            }
            var type = obj.GetType();
            var nullObject = (NullObject<T>)obj;
            if (type != typeof(NullObject<T>))
            {
                return false;
            }
            if (IsNull())
            {
                return nullObject.IsNull();
            }
            if (nullObject.IsNull())
            {
                return false;
            }
            return Item.Equals(nullObject.Item);
        }
        public override int GetHashCode()
        {
            if (isNull)
            {
                return 0;
            }
            var result = Item.GetHashCode();

            if (result >= 0)
            {
                result++;
            }
            return result;
        }
    }
}
