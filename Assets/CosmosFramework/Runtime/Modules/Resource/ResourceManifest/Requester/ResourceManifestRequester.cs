using Cosmos.WebRequest;
using System;

namespace Cosmos.Resource
{
    internal class ResourceManifestRequester
    {
        readonly IWebRequestManager webRequestManager;
        readonly Action<string, ResourceManifest> onSuccess;
        readonly Action<string, string> onFailure;
        long taskId;
        public ResourceManifestRequester(IWebRequestManager webRequestManager, Action<string, ResourceManifest> onSuccess, Action<string, string> onFailure)
        {
            this.webRequestManager = webRequestManager;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
        }
        public void OnInitialize()
        {
            webRequestManager.OnSuccessCallback += OnSuccessCallback;
            webRequestManager.OnFailureCallback += OnFailureCallback;
        }
        public void OnTerminate()
        {
            webRequestManager.OnSuccessCallback -= OnSuccessCallback;
            webRequestManager.OnFailureCallback -= OnFailureCallback;
        }
        public void StartRequestManifest(string url)
        {
            taskId = webRequestManager.AddDownloadTextTask(url);
        }
        public void StopRequestManifest()
        {
            webRequestManager.RemoveTask(taskId);
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestContext = eventArgs.GetText();
            try
            {
                var resourceManifest = ResourceUtility.Manifest.Deserialize(manifestContext, ResourceDataProxy.ManifestEncryptionKey);
                onSuccess?.Invoke(eventArgs.URL, resourceManifest);
            }
            catch (Exception e)
            {
                onFailure?.Invoke(eventArgs.URL, e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            onFailure?.Invoke(eventArgs.URL, eventArgs.ErrorMessage);
        }
    }
}
