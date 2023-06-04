using Cosmos.WebRequest;
using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    internal class ResourceManifestRequester
    {
        readonly IWebRequestManager webRequestManager;
        readonly Action<string, ResourceManifest> onSuccess;
        readonly Action<string, string> onFailure;
        Dictionary<long, string> taskKeyDict;
        public ResourceManifestRequester(IWebRequestManager webRequestManager, Action<string, ResourceManifest> onSuccess, Action<string, string> onFailure)
        {
            taskKeyDict = new Dictionary<long, string>();
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
        public void StartRequestManifest(string url, string manifestEncryptionKey)
        {
            var taskId = webRequestManager.AddDownloadTextTask(url);
            taskKeyDict.TryAdd(taskId, manifestEncryptionKey);
        }
        public void StopRequestManifest()
        {
            foreach (var tk in taskKeyDict)
            {
                webRequestManager.RemoveTask(tk.Key);
            }
            taskKeyDict.Clear();
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestContext = eventArgs.GetText();
            try
            {
                taskKeyDict.Remove(eventArgs.TaskId, out var key);
                var resourceManifest = ResourceUtility.Manifest.Deserialize(manifestContext, key);
                onSuccess?.Invoke(eventArgs.URL, resourceManifest);
            }
            catch (Exception e)
            {
                onFailure?.Invoke(eventArgs.URL, e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            taskKeyDict.Remove(eventArgs.TaskId);
            onFailure?.Invoke(eventArgs.URL, eventArgs.ErrorMessage);
        }
    }
}
