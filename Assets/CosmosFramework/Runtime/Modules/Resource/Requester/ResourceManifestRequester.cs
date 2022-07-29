using Cosmos.WebRequest;
using System;
using UnityEngine;

namespace Cosmos.Resource
{
    public class ResourceManifestRequester
    {
        public Action onStartCallback;
        public Action<float> onUpdateCallback;
        public Action<byte[]> onSuccessCallback;
        public Action<string> onFailureCallback;
        IWebRequestManager webRequestManager { get { return GameManager.GetModule<IWebRequestManager>(); } }
        Coroutine coroutine;
        public Coroutine RequestManifest(string url)
        {
            var callback = WebRequestCallback.Create(onStartCallback, onUpdateCallback, onSuccessCallback, onFailureCallback);
            coroutine = webRequestManager.RequestTextAsync(url, callback, OnResultCallback);
            return coroutine;
        }
        public void StopRequestManifest()
        {
            Utility.Unity.StopCoroutine(coroutine);
        }
        void OnResultCallback(string text)
        {
       
        }
    }
}
