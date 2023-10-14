using System;
using System.Collections.Generic;

namespace LiteMVC.Core
{
    internal class Notification
    {
        Dictionary<BindKey, IBinder> binderDict = new Dictionary<BindKey, IBinder>();
        public int BindTypeCount { get { return binderDict.Count; } }
        /// <summary>
        /// 多线程锁；
        /// </summary>
        readonly object locker = new object();
        /// <summary>
        /// 数据类型是否存在绑定；
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <returns>是否存在的结果</returns>
        public bool HasBind(Type type, string eventName)
        {
            lock (locker)
            {
                var key = new BindKey(type, eventName);
                return binderDict.ContainsKey(key);
            }
        }
        /// <summary>
        /// 绑定数据；
        /// 注意：使用匿名函数绑定，会导致解绑时地址寻址失败，尽量使用明确的函数进行绑定！
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">绑定的函数</param>
        public void Bind<T>(string eventName, Action<T> action)
        {
            var type = typeof(T);
            NotificationBinder<T> binder = default;
            var key = new BindKey(type, eventName);
            lock (locker)
            {
                if (!binderDict.ContainsKey(key))
                {
                    binder = new NotificationBinder<T>();
                    binderDict[key] = binder;
                }
            }
            binder = (NotificationBinder<T>)binderDict[key];
            binder.Bind(action);
        }
        /// <summary>
        /// 解绑数据；
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="action">解绑的函数</param>
        public void Unbind<T>(string eventName, Action<T> action)
        {
            var type = typeof(T);
            var key = new BindKey(type, eventName);
            lock (locker)
            {
                if (binderDict.ContainsKey(key))
                {
                    var ibinder = binderDict[key];
                    var binder = (NotificationBinder<T>)ibinder;
                    binder.Unbind(action);
                    if (binder.BindCount <= 0)
                    {
                        binderDict.Remove(key);
                    }
                }
            }
        }
        /// <summary>
        /// 解绑制定类型的事件；
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="eventName">事件名</param>
        public void UnbindType(Type type, string eventName)
        {
            var key = new BindKey(type, eventName);
            lock (locker)
            {
                if (binderDict.ContainsKey(key))
                {
                    var binder = binderDict[key];
                    binder.Clear();
                    binderDict.Remove(key);
                }
            }
        }
        /// <summary>
        /// 获取制定绑定类型，被绑定函数的数量；
        /// </summary>
        /// <param name="dataType">绑定的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <returns>绑定的数据量，若不存在则返回0</returns>
        public int GetBindCount(Type dataType, string eventName)
        {
            var key = new BindKey(dataType, eventName);
            lock (locker)
            {
                if (binderDict.TryGetValue(key, out var binder))
                {
                    return binder.BindCount;
                }
            }
            return 0;
        }
        /// <summary>
        /// 消息派发
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="data">数据</param>
        public void Dispatch(string eventName, object data)
        {
            var type = data.GetType();
            var key = new BindKey(type, eventName);
            lock (locker)
            {
                if (binderDict.TryGetValue(key, out var binder))
                {
                    binder.Execute(data);
                }
            }
        }
        /// <summary>
        /// 清理所有绑定；
        /// 谨慎使用！
        /// </summary>
        public void UnbindAll()
        {
            foreach (var binder in binderDict.Values)
            {
                binder.Clear();
            }
            binderDict.Clear();
        }
    }
}
