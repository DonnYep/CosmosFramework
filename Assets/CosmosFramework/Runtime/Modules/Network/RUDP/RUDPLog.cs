using System;
namespace RUDP
{
    public static class RUDPLog
    {
        public enum RUDPLoggerLevel
        {
            Trace = 0,
            Info = 1,
            None = 99,
        }
        public static Action<string,object> DebugLog=(msg, context) => Console.WriteLine($"{msg}:{context}");
        public static RUDPLoggerLevel LogLevel = RUDPLoggerLevel.Info;
        public static void Trace(string msg, object context = null)
        {
            if (LogLevel <= RUDPLoggerLevel.Trace)
                DebugLog?.Invoke(msg, context);
        }

        public static void Info(string msg, object context = null)
        {
            if (LogLevel <= RUDPLoggerLevel.Info)
                DebugLog?.Invoke(msg, context);
        }
    }
}
