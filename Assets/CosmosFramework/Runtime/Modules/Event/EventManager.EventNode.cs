using System;
using System.Collections.Generic;
namespace Cosmos.Event
{
    internal sealed partial class EventManager
    {
        private abstract class EventNodeBase
        {
            public abstract int ListenerCount { get; }
            public abstract void Clear();
        }
        private class EventNode<T> : EventNodeBase
            where T : GameEventArgs
        {
            int listenerCount = 0;
            Action<T> eventHandler;
            public override int ListenerCount
            {
                get { return listenerCount; }
            }
            public event Action<T> EventHandler
            {
                add
                {
                    eventHandler += value;
                    listenerCount++;
                }
                remove
                {
                    eventHandler -= value;
                    if (listenerCount > 0)
                        listenerCount--;
                }
            }
            public void Dispatch(T args)
            {
                eventHandler?.Invoke(args);
            }
            public override void Clear()
            {
                eventHandler = null;
                listenerCount = 0;
            }
        }
    }
}
