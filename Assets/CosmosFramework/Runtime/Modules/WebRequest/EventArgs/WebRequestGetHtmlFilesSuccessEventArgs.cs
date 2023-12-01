using System;

namespace Cosmos.WebRequest
{
    public class WebRequestGetHtmlFilesSuccessEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public WebRequestUrlFileInfo[] UrlFileInfos { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            UrlFileInfos = null;
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestGetHtmlFilesSuccessEventArgs Create(long taskId, string url, WebRequestUrlFileInfo[]  urlFileInfos, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestGetHtmlFilesSuccessEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.UrlFileInfos = urlFileInfos;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestGetHtmlFilesSuccessEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
