using System;

namespace Cosmos.WebRequest
{
    public class WebRequestGetContentLengthFailureEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public string ErrorMessage { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            ErrorMessage = string.Empty;
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestGetContentLengthFailureEventArgs Create(long taskId, string url, string errorMessage, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetContentLengthFailureEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.ErrorMessage = errorMessage;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestGetContentLengthFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
