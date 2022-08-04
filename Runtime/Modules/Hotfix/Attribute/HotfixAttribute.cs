using System;
namespace Cosmos.Hotfix
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =false,Inherited =false)]
    public class HotfixAttribute:Attribute
    {
    }
}
