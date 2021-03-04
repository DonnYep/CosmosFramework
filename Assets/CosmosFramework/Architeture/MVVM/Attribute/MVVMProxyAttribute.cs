using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Cosmos
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MVVMProxyAttribute:Attribute
    {
    }
}
