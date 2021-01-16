using System.Collections;
using System.Collections.Generic;
using System;
using Cosmos;
namespace Cosmos.Mvvm
{
    public  class ViewModel:ConcurrentSingleton<ViewModel>
    {
        protected Dictionary<string, Type> cmdTypeDict;
        protected Dictionary<Type, Queue<Command>> typeCmdQueueDict;
        protected Type cmdType = typeof(Command);
        protected View view;
        readonly protected object locker = new object();
        public ViewModel()
        {
            cmdTypeDict = new Dictionary<string, Type>();
            typeCmdQueueDict = new Dictionary<Type, Queue<Command>>();
            OnInitialization();
        }
        public virtual void RegisterCommand(string actionKey,Type cmdType)
        {
            lock (locker)
            {
                //if (!cmdTypeDict.ContainsKey(actionKey))
                //{
                //    view.AddListener(actionKey, ExecuteCommand);
                //}
                cmdTypeDict[actionKey] = cmdType;
            }
        }
        public virtual void RemoveCommand(string actionKey)
        {
            lock (locker)
            {
                if (cmdTypeDict.Remove(actionKey,out var type))
                {
                    typeCmdQueueDict.Remove(type);
                    //view.RemoveListener(actionKey, ExecuteCommand);
                }
            }
        }
        public virtual void ExecuteCommand(string actionKey, object sender, NotifyArgs notifyArgs)
        {
            Command cmd=null;
            Queue<Command> cmdQueue = null;
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
            cmd.Execute(sender, notifyArgs);
            cmdQueue.Enqueue(cmd);
        }
        public virtual bool HasCommand(string actionKey)
        {
            lock (locker)
            {
                return cmdTypeDict.ContainsKey(actionKey);
            }
        }
        protected virtual void OnInitialization()
        {
            view = View.Instance;
        }
    }
}