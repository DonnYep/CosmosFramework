using System;
using System.Collections.Generic;

namespace LiteMVC.Core
{
    internal class CommandBinder<T> : ICommandBinder
    {
        Type valueType = typeof(T);
        List<Type> bindCmds;
        public int BindCount { get { return bindCmds.Count; } }
        public Type ValueType { get { return valueType; } }
        public CommandBinder()
        {
            bindCmds = new List<Type>();
        }
        public void Bind(Type cmdType)
        {
            if (!bindCmds.Contains(cmdType))
                bindCmds.Add(cmdType);
        }
        public void Unbind(Type cmdType)
        {
            bindCmds.Remove(cmdType);
        }
        public void Execute(object data)
        {
            var length = bindCmds.Count;
            for (int i = 0; i < length; i++)
            {
                var cmdType = bindCmds[i];
                var cmd = (Command<T>)CommandPool.Acquire(cmdType);
                cmd.ExecuteCommand((T)data);
                CommandPool.Release(cmd);
            }
        }
        public bool HasBind(Type cmdType)
        {
            return bindCmds.Contains(cmdType);
        }
        public void Clear()
        {
            bindCmds.Clear();
        }
    }
}
