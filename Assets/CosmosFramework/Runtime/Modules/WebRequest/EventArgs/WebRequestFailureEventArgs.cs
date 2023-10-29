using System;
using UnityEngine.Networking;
namespace Cosmos.WebRequest
{
    public class WebRequestFailureEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public string ErrorMessage { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            ErrorMessage = string.Empty;
            WebRequest = null;
            TimeSpan = TimeSpan.Zero;
        }
        internal static WebRequestFailureEventArgs Create(long taskId, string url, string errorMessage, UnityWebRequest webRequest, TimeSpan timeSpan)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestFailureEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.ErrorMessage = errorMessage;
            eventArgs.WebRequest = webRequest;
            eventArgs.TimeSpan = timeSpan;
            return eventArgs;
        }
        internal static void Release(WebRequestFailureEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
