using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    public class WebRequestUpdateEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public float Progress { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            WebRequest = null;
            Progress = 0;
        }
        internal static WebRequestUpdateEventArgs Create(long taskId, string url, float progress, UnityWebRequest webRequest)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestUpdateEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.Progress = progress;
            eventArgs.WebRequest = webRequest;
            return eventArgs;
        }
        internal static void Release(WebRequestUpdateEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
