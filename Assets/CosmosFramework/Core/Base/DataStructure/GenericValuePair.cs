using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 双泛型值组合；此组合并非Key-Value形式，而是Value-Value形式。
    /// 比较时调用Equals方法；进行组合时注意覆写Equals；
    /// </summary>
    /// <typeparam name="TValue">泛型类型</typeparam>
    /// <typeparam name="KValue">泛型类型</typeparam>
    public struct GenericValuePair<TValue, KValue> : IEquatable<GenericValuePair<TValue, KValue>>
    {
        public TValue TVar { get; private set; }
        public KValue KVar { get; private set; }
        public GenericValuePair(TValue tVar, KValue kVar)
        {
            TVar = tVar;
            KVar = kVar;
        }
        public bool Equals(GenericValuePair<TValue, KValue> other)
        {
            return TVar.Equals(other.TVar) && KVar.Equals(other.KVar);
        }
        public static bool operator ==(GenericValuePair<TValue, KValue> a, GenericValuePair<TValue, KValue> b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(GenericValuePair<TValue, KValue> a, GenericValuePair<TValue, KValue> b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is GenericValuePair<TValue, KValue> && Equals((GenericValuePair<TValue, KValue>)obj);
        }
        public override int GetHashCode()
        {
            return TVar.GetHashCode() ^ KVar.GetHashCode();
        }
        public override string ToString()
        {
            if (TVar == null)
                throw new ArgumentNullException($"GenericValuePair: {typeof(TValue)} is  invalid");
            if (KVar == null)
                throw new ArgumentNullException($"GenericValuePair: {typeof(KValue)} is  invalid");
            return $"{typeof(TValue)}：{TVar}；{typeof(KValue)}：{KVar}";
        }
    }
}
