using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Mvvm
{
    public class Observer
    {
        /// <summary>
        /// 监听者
        /// </summary>
        protected object observer;
        /// <summary>
        /// 监听者
        /// </summary>
        public Observer(object observer)
        {
            this.observer = observer;
        }
        public virtual void NotifyObserver(object args) 
        {
            Type t = observer.GetType();
            BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            var methods= t.GetMethods();
            var length = methods.Length;
            for (int i = 0; i < length; i++)
            {
                var attList= methods[i].GetCustomAttributes<NotifyHandlerAttribute>().ToList();
                if (attList.Count > 0)
                {
                    methods[i].Invoke(observer, new object[] { args });
                }
            }
        }
    }
}
