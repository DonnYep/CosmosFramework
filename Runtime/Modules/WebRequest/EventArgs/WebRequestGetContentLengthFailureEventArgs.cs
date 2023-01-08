namespace Cosmos.WebRequest
{
    public class WebRequestGetContentLengthFailureEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public string ErrorMessage { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            ErrorMessage = string.Empty;
        }
        internal static WebRequestGetContentLengthFailureEventArgs Create(long taskId, string url, string errorMessage)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetContentLengthFailureEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.ErrorMessage = errorMessage;
            return eventArgs;
        }
        internal static void Release(WebRequestGetContentLengthFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
