using System;
using System.Collections;
using System.Collections.Generic;

namespace Cosmos.Mvvm
{
    /// <summary>
    /// ViewModel标签；
    /// 此特性不可继承，不可重复挂载；
    /// 若目标继承自ViewModel且挂载此标签，则可进行自动注册；
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple =false, Inherited =false)]
    public class CommandAttribute:Attribute
    {
        public CommandAttribute(string cmdName)
        {
            CommandName = cmdName;
        }
        public string CommandName { get; private set; }
    }
}