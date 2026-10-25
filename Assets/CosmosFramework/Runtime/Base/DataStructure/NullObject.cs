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
            //默认结构体的 isNull 为 false，必须显式构造空状态
            return new NullObject<T>(default(T), true);
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
                if (ReferenceEquals(Item, null))
                    return ReferenceEquals(nullObject.Item, null) ? 0 : -1;
                if (ReferenceEquals(nullObject.Item, null))
                    return 1;
                if (Item is IComparable comparable)
                    return comparable.CompareTo(nullObject.Item);
                return string.CompareOrdinal(Item.ToString(), nullObject.Item.ToString());
            }
            return 1;
        }
        public int CompareTo(T other)
        {
            if (ReferenceEquals(Item, null))
                return ReferenceEquals(other, null) ? 0 : -1;
            if (ReferenceEquals(other, null))
                return 1;
            if (Item is IComparable comparable)
                return comparable.CompareTo(other);
            return string.CompareOrdinal(Item.ToString(), other.ToString());
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return IsNull() || ReferenceEquals(Item, null);
            }
            var type = obj.GetType();
            if (type != typeof(NullObject<T>))
            {
                return false;
            }
            var nullObject = (NullObject<T>)obj;
            if (IsNull() || ReferenceEquals(Item, null))
            {
                return nullObject.IsNull() || ReferenceEquals(nullObject.Item, null);
            }
            if (nullObject.IsNull() || ReferenceEquals(nullObject.Item, null))
            {
                return false;
            }
            return Item.Equals(nullObject.Item);
        }
        public override int GetHashCode()
        {
            if (isNull || ReferenceEquals(Item, null))
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
