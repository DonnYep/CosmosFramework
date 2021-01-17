using System.Collections;
using System.Collections.Generic;
using System;
namespace Cosmos.Mvvm
{
    public class View : ConcurrentSingleton<View>
    {
        protected Dictionary<string, Mediator> mediatorDict;

        protected readonly object locker = new object();
        public View()
        {
            mediatorDict = new Dictionary<string, Mediator>();

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
                         ViewModel.Instance. AddListener(bindedKeys[i], mediator.HandleEvent);
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
                        ViewModel.Instance.RemoveListener(bindedKeys[i], mediator.HandleEvent);
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
        public void Dispatch(string actionKey, object sender, NotifyArgs notifyArgs)
        {
            ViewModel.Instance.Dispatch(actionKey, sender, notifyArgs);
        }
        protected virtual void OnInitialization(){}
    }
}