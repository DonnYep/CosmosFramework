using Cosmos.WebRequest;
using System;

namespace Cosmos.Resource
{
    public class ResourceManifestRequester
    {
        readonly IWebRequestManager webRequestManager;
        readonly Action<ResourceManifest> onSuccess;
        readonly Action<string> onFailure;
        long taskId;
        public ResourceManifestRequester(IWebRequestManager webRequestManager, Action<ResourceManifest> onSuccess, Action<string> onFailure)
        {
            this.webRequestManager = webRequestManager;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;
        }
        public void StartRequestManifest(string url)
        {
            webRequestManager.AddDownloadTextTask(url);
            webRequestManager.OnSuccessCallback += OnSuccessCallback;
            webRequestManager.OnFailureCallback += OnFailureCallback;
        }
        public void StopRequestManifest()
        {
            webRequestManager.RemoveTask(taskId);
            webRequestManager.OnSuccessCallback -= OnSuccessCallback;
            webRequestManager.OnFailureCallback -= OnFailureCallback;
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestJson = eventArgs.GetText();
            try
            {
                var resourceManifest = Utility.Json.ToObject<ResourceManifest>(manifestJson);
                onSuccess?.Invoke(resourceManifest);
            }
            catch (Exception e)
            {
                onFailure?.Invoke(e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            onFailure?.Invoke(eventArgs.ErrorMessage);
        }
    }
}
