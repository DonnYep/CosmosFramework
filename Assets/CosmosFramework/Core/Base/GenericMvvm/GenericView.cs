using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.GenericMvvm
{
    public class View<T>
        where T: class,IView
    {
        T viewContext;
        public T ViewContext { get { return viewContext; } }
        Type viewContextType = typeof(T);
        public Type ViewContextType { get { return viewContextType; } }
        public string Name { get; }
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
        public void HandleEvent(string cmdName, object data = null)
        {
            viewContext.HandleEvent(cmdName, data);
        }
    }
}
