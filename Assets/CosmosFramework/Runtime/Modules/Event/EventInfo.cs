using System;
using System.Runtime.InteropServices;

namespace Cosmos.Event
{
    [StructLayout(LayoutKind.Auto)]
    public struct EventInfo
    {
        public int ListenerCount { get; private set; }
        public Type Type { get; private set; }
        public static readonly EventInfo Default = new EventInfo();
        internal static EventInfo Create(Type type, int listenerCount)
        {
            var ei = new EventInfo();
            ei.Type = type;
            ei.ListenerCount = listenerCount;
            return ei;
        }
    }
}
