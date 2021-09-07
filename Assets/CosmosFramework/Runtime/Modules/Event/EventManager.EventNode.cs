using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
namespace Cosmos.Event
{
    internal sealed partial class EventManager
    {
        private class EventNode
        {
            int listenerCount = 0;
            EventHandler<GameEventArgs> eventHandler;
            public int ListenerCount { get { return listenerCount; } }
            public event EventHandler<GameEventArgs> EventHandler
            {
                add { eventHandler += value; listenerCount++; }
                remove { eventHandler -= value; if (listenerCount > 0) listenerCount--; }
            }
            public void DispatchEvent(object sender, GameEventArgs args)
            {
                eventHandler.Invoke(sender, args);
            }
            public void Clear()
            {
                eventHandler = null;
                listenerCount = 0;
            }
        }
    }
}
