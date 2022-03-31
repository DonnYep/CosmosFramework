using System;

namespace Cosmos
{
    /// <summary>
    /// 具体实现提供者；
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class ImplementerAttribute:Attribute
    {
    }
}
