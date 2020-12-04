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
        public CommandAttribute(string cmdName):this(cmdName,null){}
        public CommandAttribute(Type cmdType) : this(string.Empty, cmdType) { }
        public CommandAttribute(string cmdName,Type cmdType)
        {
            CommandName = cmdName;
            CommandType= cmdType;
        }
        public string CommandName { get; private set; }
        public Type CommandType { get; private set; }
    }
}