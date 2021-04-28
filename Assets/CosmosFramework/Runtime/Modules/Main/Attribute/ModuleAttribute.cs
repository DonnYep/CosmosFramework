using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    /// <summary>
    /// 模块标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 模块优先级；
        /// </summary>
        public int Priority { get; set; }
        public ModuleAttribute()
        {
            Priority = 100;
        }
    }
}