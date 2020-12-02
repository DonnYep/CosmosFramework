using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    /// <summary>
    /// 自定义模块特性；
    /// 优先级：内置模块->扩展模块；
    /// 挂载此特性的模块必须继承自Module，实现之后模块将享有与内嵌模块同生命周期等级；
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExternalModuleAttribute : Attribute
    {
        /// <summary>
        /// 模块优先级；
        /// </summary>
        public int Priority { get; set; }
        public ExternalModuleAttribute()
        {
            Priority = 100;
        }
    }
}
