using System.Runtime.InteropServices;

namespace Cosmos.Event
{
    [StructLayout(LayoutKind.Auto)]
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
