using System;

namespace kcp 
{
    public static class KCPLog
    {
        public static Action<string> Info = Console.WriteLine;
        public static Action<string> Warning = Console.WriteLine;
        public static Action<string> Error = Console.Error.WriteLine;
    }
}
