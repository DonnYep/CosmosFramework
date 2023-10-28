using Cosmos.WebRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Resource
{
    public class ResourceDownloader
    {
        readonly IWebRequestManager webRequestManager;
       readonly Dictionary<long, ResourceDownloadTask> taskDict;
        readonly Action<string, ResourceManifest> onSuccess;
        readonly Action<string, string> onFailure;
        public ResourceDownloader(IWebRequestManager webRequestManager)
        {
            this.webRequestManager = webRequestManager;
            taskDict = new Dictionary<long, ResourceDownloadTask>();
        }
        public void OnInitialize()
        {
            webRequestManager.OnSuccessCallback += OnSuccessCallback;
            webRequestManager.OnFailureCallback += OnFailureCallback;
            webRequestManager.OnUpdateCallback += OnUpdateCallback; 
        }
        public void OnTerminate()
        {
            webRequestManager.OnSuccessCallback -= OnSuccessCallback;
            webRequestManager.OnFailureCallback -= OnFailureCallback;
            webRequestManager.OnUpdateCallback -= OnUpdateCallback;
        }
        public void AddDownloadTask(ResourceDownloadTask downloadTask)
        {
        }
        public void RemoveTask(ResourceDownloadTask downloadTask)
        {

        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestContext = eventArgs.GetText();
            try
            {
                taskDict.Remove(eventArgs.TaskId, out var key);

            }
            catch (Exception e)
            {
                onFailure?.Invoke(eventArgs.URL, e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            taskDict.Remove(eventArgs.TaskId);
            onFailure?.Invoke(eventArgs.URL, eventArgs.ErrorMessage);
        }
        private void OnUpdateCallback(WebRequestUpdateEventArgs eventArgs)
        {

        }
    }
}
