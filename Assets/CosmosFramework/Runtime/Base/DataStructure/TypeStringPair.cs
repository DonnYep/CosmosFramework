using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    /// <summary>
    /// Type类型与名称组合
    /// </summary>
    public struct TypeStringPair : IEquatable<TypeStringPair>
    {
        Type type;
        string _string;
        public Type Type { get { return type; } }
        public string String { get { return _string; } }
        public TypeStringPair(Type type, string str)
        {
            this.type = type;
            this._string = str;
        }
        public TypeStringPair(Type type) : this(type, string.Empty) { }
        public void Clear()
        {
            type = default;
            _string = default;
        }
        public bool Equals(TypeStringPair other)
        {
            return type == other.type && _string == other._string;
        }
        public static bool operator ==(TypeStringPair a, TypeStringPair b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(TypeStringPair a, TypeStringPair b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is TypeStringPair && Equals((TypeStringPair)obj);
        }
        public override int GetHashCode()
        {
            return type.GetHashCode() ^ _string.GetHashCode();
        }
        public override string ToString()
        {
            if (type == null)
                throw new ArgumentNullException("TypeNamePair : Type is invalid");
            var typeName = type.FullName;
            return string.IsNullOrEmpty(_string) ? typeName : Utility.Text.Format("{0}.{1}", typeName, _string);
        }
    }
}