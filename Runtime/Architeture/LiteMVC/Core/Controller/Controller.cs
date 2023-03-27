using System;
using System.Collections.Generic;
namespace LiteMVC.Core
{
    public class Controller
    {
        /// <summary>
        /// DataType===ICommandBinder
        /// </summary>
        Dictionary<BindKey, ICommandBinder> cmdBindDict = new Dictionary<BindKey, ICommandBinder>();
        readonly object locker = new object();
        public int TypeBindCount { get { return cmdBindDict.Count; } }
        /// <summary>
        /// 绑定command
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">command类型</param>
        public void BindCommand<T>(string eventName, Type cmdType)
        {
            var dataType = typeof(T);
            var baseCmdType = typeof(Command<>).MakeGenericType(dataType);
            if (!baseCmdType.IsAssignableFrom(cmdType))
                throw new Exception($"{cmdType} is not inherit form Command<T>");
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                if (!cmdBindDict.TryGetValue(key, out var binder))
                {
                    binder = new CommandBinder<T>();
                    cmdBindDict[key] = binder;
                }
                binder.Bind(cmdType);
            }
        }
        /// <summary>
        ///注销对指定类型的数据的指定cmd绑定
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">cmd类型</param>
        public void UnbindCommand(Type dataType, string eventName, Type cmdType)
        {
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                if (cmdBindDict.TryGetValue(key, out var binder))
                {
                    binder.Unbind(cmdType);
                }
            }
        }
        /// <summary>
        /// 解除指定数据类型的绑定；
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        public void UnbindCommands(Type dataType, string eventName)
        {
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                if (cmdBindDict.ContainsKey(key))
                {
                    var binder = cmdBindDict[key];
                    binder.Clear();
                    cmdBindDict.Remove(key);
                }
            }
        }
        /// <summary>
        /// 是否注册了订阅的数据类型
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <param name="cmdType">command类型</param>
        /// <returns>是否订阅</returns>
        public bool HasCommandBind(Type dataType, string eventName, Type cmdType)
        {
            var baseCmdType = typeof(Command<>).MakeGenericType(dataType);
            if (!baseCmdType.IsAssignableFrom(cmdType))
                throw new Exception($"{cmdType} is not inherit form Command<>");
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                if (cmdBindDict.TryGetValue(key, out var binder))
                {
                    return binder.HasBind(cmdType);
                }
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否存在对指定数据类型的绑定
        /// </summary>
        /// <param name="dataType">订阅的数据类型</param>
        /// <param name="eventName">事件名</param>
        /// <returns>绑定结果</returns>
        public bool HasBind(Type dataType, string eventName)
        {
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                return cmdBindDict.ContainsKey(key);
            }
        }
        /// <summary>
        /// 派发消息；
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="data">消息对象</param>
        public void Dispatch(string eventName, object data)
        {
            var dataType = data.GetType();
            lock (locker)
            {
                var key = new BindKey(dataType, eventName);
                if (cmdBindDict.TryGetValue(key, out var binder))
                {
                    binder.Execute(data);
                }
            }
        }
    }
}
