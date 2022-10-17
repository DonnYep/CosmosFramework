using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    internal class WebRequestTask : IReference
    {
        public long TaskId { get; private set; }
        public string URL { get; private set; }
        public UnityWebRequest UnityWebRequest { get; private set; }
        public WebRequestType WebRequestType { get; private set; }
        public bool TaskAvailable { get; set; }

        static long taskIndex = 0;
        public void Release()
        {
            TaskId = -1;
            URL = string.Empty;
            UnityWebRequest = null;
            WebRequestType = WebRequestType.None;
            TaskAvailable = false;
        }
        public static WebRequestTask Create(string url, UnityWebRequest webRequest, WebRequestType requestType)
        {
            var requestTask = ReferencePool.Acquire<WebRequestTask>();
            requestTask.TaskId = GetTaskId();
            requestTask.URL = url;
            requestTask.UnityWebRequest = webRequest;
            requestTask.WebRequestType = requestType;
            requestTask.TaskAvailable = true;
            return requestTask;
        }
        public static void Release(WebRequestTask requestTask)
        {
            ReferencePool.Release(requestTask);
        }
        static long GetTaskId()
        {
            taskIndex++;
            if (taskIndex > long.MaxValue)
                taskIndex = 0;
            return taskIndex;
        }
    }
}
