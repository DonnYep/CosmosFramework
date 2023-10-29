using System;

namespace Cosmos.WebRequest
{
    public class WebRequestGetContentLengthSuccessEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public long ContentLength { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            ContentLength = 0;
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestGetContentLengthSuccessEventArgs Create(long taskId, string url, long contentLength, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetContentLengthSuccessEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.ContentLength = contentLength;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestGetContentLengthSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
