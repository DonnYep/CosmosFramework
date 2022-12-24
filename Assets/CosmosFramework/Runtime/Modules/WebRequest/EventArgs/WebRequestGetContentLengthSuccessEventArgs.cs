namespace Cosmos.WebRequest
{
    public class WebRequestGetContentLengthSuccessEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public long ContentLength { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            ContentLength = 0;
        }
        internal static WebRequestGetContentLengthSuccessEventArgs Create(long taskId, string url, long contentLength)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetContentLengthSuccessEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.ContentLength= contentLength;
            return eventArgs;
        }
        internal static void Release(WebRequestGetContentLengthSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
