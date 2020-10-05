using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos
{
    /// <summary>
    /// 模块标记特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    internal class ModuleAttribute : Attribute
    {
    }
}