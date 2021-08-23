using System.Collections.Generic;
namespace PureMVC
{
    public class View : MVCSingleton<View>
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
                if (mediatorDict.ContainsKey(mediator.MediatorName))
                    return;
                mediatorDict.Add(mediator.MediatorName, mediator);
                var bindedKeys = mediator.EventKeys;
                if (bindedKeys != null)
                {
                    var length = bindedKeys.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Controller.Instance.AddListener(bindedKeys[i], mediator.HandleEvent);
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
                if (!mediatorDict.ContainsKey(mediatorName))
                    return;
                mediator = mediatorDict[mediatorName];
                mediatorDict.Remove(mediatorName);
                var bindedKeys = mediator.EventKeys;
                if (bindedKeys != null)
                {
                    var length = bindedKeys.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Controller.Instance.RemoveListener(bindedKeys[i], mediator.HandleEvent);
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
        public void Dispatch(INotifyArgs notifyArgs)
        {
            Controller.Instance.Dispatch(notifyArgs);
        }
        protected virtual void OnInitialization() { }
    }
}