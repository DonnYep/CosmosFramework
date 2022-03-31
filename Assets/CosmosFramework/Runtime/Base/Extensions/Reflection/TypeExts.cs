using System;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos
{
    public static class TypeExts
    {
        public static bool IsAssignableTo(this Type @this, Type type)
        {
            return type.IsAssignableFrom(@this);
        }
        public static bool Is(this Type @this, Type type)
        {
            return @this.IsAssignableTo(type);
        }
        public static bool Is<T>(this Type @this)
        {
            return @this.IsAssignableTo(typeof(T));
        }
        public static string GetShortAssemblyName(this Type @this)
        {
            return @this.Assembly.GetName().Name;
        }
        public static IEnumerable<Type> GetConcreteSubtypes(this Type @this, IEnumerable<Type> types = null)
        {
            if (types == null)
                types = @this.Assembly.GetTypes();
            return @this.IsClass // else is interface
                ? types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(@this))
                : types.Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(@this));
        }
    }
}
