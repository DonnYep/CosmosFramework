using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public class ObjectKey<T>:IReference
        where T:IObject
    {
        Type type;
        public Type Type { get { return type; } }
        TypeNamePair typeNamePair;
        public ObjectKey(string name)
        {
            type = typeof(T);
            typeNamePair = new TypeNamePair(type, name);
        }
        public ObjectKey()
        {
            type = typeof(T);
            typeNamePair = new TypeNamePair(type);
        }
        public void SetValue(string name)
        {
            type = typeof(T);
            typeNamePair.SetValue(type, name);
        }
        public TypeNamePair GetValue()
        {
            return typeNamePair;
        }
        public override bool Equals(object obj)
        {
            return obj is ObjectKey<T> && typeNamePair .Equals(((ObjectKey<T>)obj).GetValue());
        }
        public void Clear()
        {
            typeNamePair.Clear();
        }
        public override string ToString()
        {
            return typeNamePair.ToString();
        }
    }
}
