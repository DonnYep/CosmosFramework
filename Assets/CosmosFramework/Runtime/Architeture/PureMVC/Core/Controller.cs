using System.Collections.Generic;
using System;
namespace PureMVC
{
    public class Controller : MVCSingleton<Controller>
    {
        protected Dictionary<string, Type> cmdTypeDict;
        protected Dictionary<Type, Queue<Command>> typeCmdQueueDict;
        protected Dictionary<string, IList<Action<INotifyArgs>>> eventDict;

        protected Type cmdType = typeof(Command);
        readonly protected object locker = new object();
        public Controller()
        {
            cmdTypeDict = new Dictionary<string, Type>();
            typeCmdQueueDict = new Dictionary<Type, Queue<Command>>();
            eventDict = new Dictionary<string, IList<Action<INotifyArgs>>>();
            OnInitialization();
        }
        /// <summary>
        /// 注册Command
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
                if (cmdTypeDict.ContainsKey(actionKey))
                {
                    var type = cmdTypeDict[actionKey];
                    cmdTypeDict.Remove(actionKey);
                    typeCmdQueueDict.Remove(type);
                }
            }
        }
        /// <summary>
        /// 进行消息派发；
        /// </summary>
        internal virtual void Dispatch(INotifyArgs notifyArgs)
        {
            var actionKey = notifyArgs.NotifyName;
            Command cmd = null;
            Queue<Command> cmdQueue = null;
            IList<Action<INotifyArgs>> handlerList = null;
            lock (locker)
            {
                if (cmdTypeDict.TryGetValue(actionKey, out var type))
                {
                    if (typeCmdQueueDict.TryGetValue(type, out cmdQueue))
                    {
                        if (cmdQueue.Count > 0)
                            cmd = cmdQueue.Dequeue();
                        else
                            cmd = Activator.CreateInstance(type) as Command;
                    }
                    else
                    {
                        cmdQueue = new Queue<Command>();
                        cmd = Activator.CreateInstance(type) as Command;
                    }
                }
                eventDict.TryGetValue(actionKey, out handlerList);
            }
            cmd?.ExecuteCommand(notifyArgs);
            if (handlerList != null)
            {
                var length = handlerList.Count;
                for (int i = 0; i < length; i++)
                {
                    handlerList[i]?.Invoke(notifyArgs);
                }
            }
            cmdQueue?.Enqueue(cmd);
        }
        internal virtual void AddListener(string actionKey, Action<INotifyArgs> notifyHandler)
        {
            lock (locker)
            {
                IList<Action<INotifyArgs>> handlerList;
                if (!eventDict.TryGetValue(actionKey, out handlerList)) 
                {
                    handlerList = new List<Action<INotifyArgs>>();
                    eventDict.Add(actionKey, handlerList);
                }
                if (!handlerList.Contains(notifyHandler))
                    handlerList.Add(notifyHandler);
            }
        }
        internal virtual void RemoveListener(string actionKey, Action<INotifyArgs> notifyHandler)
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