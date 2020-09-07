using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    /// <summary>
    /// Type类型与名称组合
    /// </summary>
    public struct TypeNamePair : IEquatable<TypeNamePair>
    {
        Type type;
        string name;
        public Type Type { get { return type; } }
        public string Name { get { return name; } }
        public TypeNamePair(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }
        public void SetValue(Type type, string name)
        {
            Clear();
            this.type = type;
            this.name = name;
        }
        public void SetValue(Type type)
        {
            Clear();
            SetValue(type, string.Empty);
        }
        public void Clear()
        {
            type = default;
            name = default;
        }
        public TypeNamePair(Type type) : this(type, string.Empty){}
        public bool Equals(TypeNamePair other)
        {
            return type == other.type && name == other.name;
        }
        public static bool operator == (TypeNamePair a,TypeNamePair b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(TypeNamePair a, TypeNamePair b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is TypeNamePair && Equals((TypeNamePair)obj);
        }
        public override int GetHashCode()
        {
            return type.GetHashCode() ^ name.GetHashCode();
        }
        public override string ToString()
        {
            if (type == null)
                throw new ArgumentNullException("TypeNamePair : Type is invalid");
            var typeName = type.FullName;
            return string.IsNullOrEmpty(name) ? typeName : Utility.Text.Format("{0}.{1}",typeName,name);
        }
    }
}