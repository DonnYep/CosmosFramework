using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Cosmos.Mvvm
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MVVMProxyAttribute:Attribute
    {
    }
}
