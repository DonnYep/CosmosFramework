using Cosmos.WebRequest;
using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    internal class ResourceManifestRequester
    {
        internal struct ManifestRequestInfo
        {
            public string ManifestEncryptionKey;
            public string BundlePath;

            public ManifestRequestInfo(string manifestEncryptionKey, string bundlePath)
            {
                ManifestEncryptionKey = manifestEncryptionKey;
                BundlePath = bundlePath;
            }
        }

        readonly IWebRequestManager webRequestManager;
        readonly Action<long, string, string, ResourceManifest> onSuccess;
        readonly Action<long, string, string> onFailure;
        /// <summary>
        /// taskId===ManifestRequestInfo
        /// </summary>
        readonly Dictionary<long, ManifestRequestInfo> taskIdKeyDict;
        public ResourceManifestRequester(IWebRequestManager webRequestManager, Action<long, string, string, ResourceManifest> onSuccess, Action<long, string, string> onFailure)
        {

            taskIdKeyDict = new Dictionary<long, ManifestRequestInfo>();
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
        public long StartRequestManifest(string manifestUrl, string manifestEncryptionKey, string bundlePath)
        {
            var taskId = webRequestManager.AddDownloadTextTask(manifestUrl);
            var pair = new ManifestRequestInfo(manifestEncryptionKey, bundlePath);
            taskIdKeyDict.TryAdd(taskId, pair);
            return taskId;
        }
        public void StopRequestManifest()
        {
            foreach (var tk in taskIdKeyDict)
            {
                webRequestManager.RemoveTask(tk.Key);
            }
            taskIdKeyDict.Clear();
        }
        void OnSuccessCallback(WebRequestSuccessEventArgs eventArgs)
        {
            var manifestContext = eventArgs.GetText();
            try
            {
                taskIdKeyDict.Remove(eventArgs.TaskId, out var pair);
                var key = pair.ManifestEncryptionKey;
                var bundlePath = pair.BundlePath;
                var resourceManifest = ResourceUtility.Manifest.DeserializeManifest(manifestContext, key);
                onSuccess?.Invoke(eventArgs.TaskId, eventArgs.URL, bundlePath, resourceManifest);
            }
            catch (Exception e)
            {
                onFailure?.Invoke(eventArgs.TaskId, eventArgs.URL, e.ToString());
            }
        }
        void OnFailureCallback(WebRequestFailureEventArgs eventArgs)
        {
            taskIdKeyDict.Remove(eventArgs.TaskId);
            onFailure?.Invoke(eventArgs.TaskId, eventArgs.URL, eventArgs.ErrorMessage);
        }
    }
}
