using System;

namespace Cosmos.WebRequest
{
    public class WebRequestAllTaskCompleteEventArgs : GameEventArgs
    {
        /// <summary>
        /// 所有任务完成的耗时
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }
        public override void Release()
        {
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestAllTaskCompleteEventArgs Create(TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestAllTaskCompleteEventArgs>();
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestAllTaskCompleteEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
