using System;

namespace Cosmos.WebRequest
{
    public class WebRequestGetHtmlFilesFailureEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public WebRequestUrlFileInfo[] UrlFileInfos { get; private set; }
        public string[] ErrorMessages { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            UrlFileInfos = null;
            TimeSpan = TimeSpan.Zero;
            ErrorMessages = null;
        }
        internal static WebRequestGetHtmlFilesFailureEventArgs Create(long taskId, string url, WebRequestUrlFileInfo[] urlFileInfos, string[] errorMessages, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetHtmlFilesFailureEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.UrlFileInfos = urlFileInfos;
            eventArgs.ErrorMessages = errorMessages;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestGetHtmlFilesFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
