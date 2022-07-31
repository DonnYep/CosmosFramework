using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    public class WebRequestStartEventArgs : GameEventArgs
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public override void Release()
        {
            TaskId = 0;
            URL = string.Empty;
            WebRequest = null;
        }
        internal static WebRequestStartEventArgs Create(long taskId, string url, UnityWebRequest webRequest)
        {
            var eventArgs = ReferencePool.Acquire<WebRequestStartEventArgs>();
            eventArgs.TaskId = taskId;
            eventArgs.URL = url;
            eventArgs.WebRequest = webRequest;
            return eventArgs;
        }
        internal static void Release(WebRequestStartEventArgs eventArgs)
        {
            ReferencePool.Release(eventArgs);
        }
    }
}
