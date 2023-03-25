using System;

namespace LiteMVC.Core
{
    /// <summary>
    /// Type类型与string组合
    /// </summary>
    public class BindKey : IEquatable<BindKey>
    {
        Type type;
        string name;
        public Type Type { get { return type; } }
        public string Name{ get { return name; } }
        public BindKey(Type type, string str)
        {
            this.type = type;
            this.name= str;
        }
        public BindKey(Type type) : this(type, string.Empty) { }
        public void Clear()
        {
            type = default;
            name= default;
        }
        public bool Equals(BindKey other)
        {
            return type == other.type && name == other.name;
        }
        public static bool operator ==(BindKey lhs, BindKey rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(BindKey lhs, BindKey rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object obj)
        {
            return obj is BindKey && Equals((BindKey)obj);
        }
        public override int GetHashCode()
        {
            return type.GetHashCode() ^ name.GetHashCode();
        }
        public override string ToString()
        {
            if (type == null)
                throw new ArgumentNullException("BindKey: Type is invalid");
            var typeName = type.FullName;
            return string.IsNullOrEmpty(name) ? typeName : $"{typeName}.{name}";
        }
    }
}
