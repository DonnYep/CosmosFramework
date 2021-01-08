using System.Collections;
using System.Collections.Generic;
namespace Cosmos.Mvvm
{
    public abstract class View 
    {
        public object ViewEntity { get; set; }
        public abstract string CommandKey { get; }
        protected List<string> cmdKeyList = new List<string>();
        public void ExecuteEvent(string cmdKey, object data)
        {
            if (cmdKeyList.Contains(cmdKey))
            {
                HandleEvent(cmdKey, data);
            }
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        public virtual void BindCommand() { }
        protected abstract void HandleEvent(string cmdKey, object data = null);
        protected T GetCommand<T>(string cmdKey)
            where T : Controller
        {
            return MVC.GetCommand<T>(cmdKey);
        }
        protected void Fire(string cmdKey, object data = null)
        {
            MVC.Fire(cmdKey, data);
        }
    }
}