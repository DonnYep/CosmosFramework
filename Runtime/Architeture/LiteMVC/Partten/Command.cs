using LiteMVC.Core;
using System;

namespace LiteMVC
{
    /// <summary>
    /// 业务处理层；
    /// 注意，此层是无状态的类，仅仅做数据处理，勿存储状态；
    /// </summary>
    public abstract class Command<T> : ICommand
    {
        public virtual Type BindType { get { return typeof(T); } }
        public abstract void ExecuteCommand(T data);
    }
}