using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Event
{
    public struct EventInfo
    {
        public string EventKey { get; private set; }
        public int ListenerCount { get; private set; }
        public static readonly EventInfo Default = new EventInfo();
        internal static EventInfo Create(string eventKey, int listenerCount)
        {
            var ei = new EventInfo();
            ei.EventKey = eventKey;
            ei.ListenerCount = listenerCount;
            return ei;
        }
    }
}
