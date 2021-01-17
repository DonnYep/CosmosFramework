using System.Collections;
using System.Collections.Generic;
using System;
using Cosmos;
namespace Cosmos.Mvvm
{
    /// <summary>
    /// MVVM架构中，VM占有较大的逻辑比重，因此事件派发主要由VM进行控制；
    /// </summary>
    public class ViewModel : ConcurrentSingleton<ViewModel>
    {
        protected Dictionary<string, Type> cmdTypeDict;
        protected Dictionary<Type, Queue<Command>> typeCmdQueueDict;
        protected Dictionary<string, IList<EventHandler<NotifyArgs>>> eventDict;

        protected Type cmdType = typeof(Command);
        readonly protected object locker = new object();
        public ViewModel()
        {
            cmdTypeDict = new Dictionary<string, Type>();
            typeCmdQueueDict = new Dictionary<Type, Queue<Command>>();
            eventDict = new Dictionary<string, IList<EventHandler<NotifyArgs>>>();

            OnInitialization();
        }
        /// <summary>
        /// 注册command
        /// </summary>
        /// <param name="actionKey">事件码</param>
        /// <param name="cmdType">注册的cmd类型</param>
        public virtual void RegisterCommand(string actionKey, Type cmdType)
        {
            if (!cmdType.IsAssignableFrom(cmdType))
            {
                throw new ArgumentException($"ActionKey:{actionKey},Command Type:{cmdType} is not inherit from Command!");
            }
            lock (locker)
            {
                cmdTypeDict[actionKey] = cmdType;
            }
        }
        public virtual void RemoveCommand(string actionKey)
        {
            lock (locker)
            {
                if (cmdTypeDict.Remove(actionKey, out var type))
                {
                    typeCmdQueueDict.Remove(type);
                }
            }
        }
        /// <summary>
        /// 进行消息派发；
        /// MVVM架构中,VM占主导地位，因此执行顺序为VM>V
        /// </summary>
        /// <param name="actionKey">消息码</param>
        /// <param name="sender">标准事件模型中的发送者</param>
        /// <param name="notifyArgs">消息模型</param>
        internal virtual void Dispatch(string actionKey, object sender, NotifyArgs notifyArgs)
        {
            Command cmd = null;
            Queue<Command> cmdQueue = null;
            IList<EventHandler<NotifyArgs>> handlerList = null;
            lock (locker)
            {
                if (cmdTypeDict.TryGetValue(actionKey, out var type))
                {
                    if (typeCmdQueueDict.TryGetValue(type, out var queue))
                    {
                        if (queue.Count > 0)
                            cmd = queue.Dequeue();
                        else
                            cmd = Activator.CreateInstance(type) as Command;
                    }
                }
                eventDict.TryGetValue(actionKey, out handlerList);
            }
            cmd.ExecuteCommand(sender, notifyArgs);
            var length = handlerList.Count;
            for (int i = 0; i < length; i++)
            {
                handlerList[i].Invoke(sender, notifyArgs);
            }
            cmdQueue.Enqueue(cmd);
        }
        internal virtual void AddListener(string actionKey, EventHandler<NotifyArgs> notifyHandler)
        {
            lock (locker)
            {
                IList<EventHandler<NotifyArgs>> handlerList;
                if (!eventDict.TryGetValue(actionKey, out handlerList))
                    handlerList = new List<EventHandler<NotifyArgs>>();
                if (!handlerList.Contains(notifyHandler))
                    handlerList.Add(notifyHandler);
            }
        }
        internal virtual void RemoveListener(string actionKey, EventHandler<NotifyArgs> notifyHandler)
        {
            lock (locker)
            {
                if (eventDict.TryGetValue(actionKey, out var handlerList))
                {
                    if (handlerList.Contains(notifyHandler))
                        handlerList.Remove(notifyHandler);
                    if (handlerList.Count <= 0)
                        eventDict.Remove(actionKey);
                }
            }
        }
        public virtual bool HasCommand(string actionKey)
        {
            lock (locker)
            {
                return cmdTypeDict.ContainsKey(actionKey);
            }
        }
        protected virtual void OnInitialization() { }
    }
}