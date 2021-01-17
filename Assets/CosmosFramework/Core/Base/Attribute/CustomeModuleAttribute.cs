using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 外源扩展模块特性；
    /// 挂载此特性的模块必须继承自Module，实现之后模块将享有与内嵌模块同生命周期等级；
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomeModuleAttribute:Attribute
    {
    }
}
