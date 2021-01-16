using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Mvvm
{
    public class View : ConcurrentSingleton<View>
    {
        protected Dictionary<string, Mediator> mediatorDict;

        protected Dictionary<string, IList<EventHandler<NotifyArgs>>> eventDict;

        protected readonly object locker = new object();
        public View()
        {
            mediatorDict = new Dictionary<string, Mediator>();
            eventDict = new Dictionary<string, IList<EventHandler<NotifyArgs>>>();
        }
        public virtual void AddListener(string actionKey, EventHandler<NotifyArgs> notifyHandler)
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
        public virtual void RemoveListener(string actionKey, EventHandler<NotifyArgs> notifyHandler)
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
        public virtual void Dispatch(string actionKey,object sender, NotifyArgs notifyArgs)
        {
            lock (locker)
            {
                ViewModel.Instance.ExecuteCommand(actionKey, sender, notifyArgs);
                if (eventDict.TryGetValue(actionKey, out var handlerList))
                {
                    var length = handlerList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        handlerList[i].Invoke(sender,notifyArgs);
                    }
                }
            }
        }
        public virtual void RegisterMediator(Mediator mediator)
        {
            lock (locker)
            {
                if (!mediatorDict.ContainsKey(mediator.MediatorName))
                {
                    mediatorDict.Add(mediator.MediatorName, mediator);
                    var bindedKeys = mediator.EventKeys;
                    var length = bindedKeys.Count;
                    for (int i = 0; i < length; i++)
                    {
                        AddListener(bindedKeys[i], mediator.HandleEvent);
                    }
                }
            }
            mediator.OnRegister();
        }
        public virtual void RemoveMediator(string mediatorName)
        {
            Mediator mediator = null;
            lock (locker)
            {
                if (mediatorDict.ContainsKey(mediatorName))
                {
                    mediatorDict.Remove(mediatorName, out mediator);
                    var bindedKeys = mediator.EventKeys;
                    var length = bindedKeys.Count;
                    for (int i = 0; i < length; i++)
                    {
                        RemoveListener(bindedKeys[i], mediator.HandleEvent);
                    }
                }
            }
            mediator.OnRemove();
        }
        public virtual Mediator PeekMediator(string mediatorName)
        {
            lock (locker)
            {
                if (!mediatorDict.ContainsKey(mediatorName)) return null;
                return mediatorDict[mediatorName];
            }
        }
        public virtual bool HasMediator(string mediatorName)
        {
            lock (locker)
            {
                return mediatorDict.ContainsKey(mediatorName);
            }
        }
        protected virtual void OnInitialization(){}
    }
}