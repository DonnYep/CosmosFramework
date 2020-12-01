using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public abstract class View 
    {
        public abstract string Name { get; }
        protected List<string> cmdNameList = new List<string>();
        public void ExecuteEvent(string cmdName, object data)
        {
            if (cmdNameList.Contains(cmdName))
            {
                HandleEvent(cmdName, data);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        public virtual void RegisterEvent() { }
        protected abstract void HandleEvent(string cmdName, object data = null);
        protected T GetCommand<T>(string cmdName)
            where T : Command
        {
            return MVC.GetCommand<T>(cmdName);
        }
        protected void SendEvent(string cmdName, object data = null)
        {
            MVC.SendEvent(cmdName, data);
        }
    }
}