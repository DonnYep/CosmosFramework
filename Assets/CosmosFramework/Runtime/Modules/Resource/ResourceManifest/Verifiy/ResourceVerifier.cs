using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using UnityEngine;

namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// 资源文件校验器；
    /// </summary>
    public class ResourceVerifier
    {
        Action<ResourceVerifiyInfo[]> onVerified;
        public event Action<ResourceVerifiyInfo[]> OnVerified
        {
            add { onVerified += value; }
            remove { onVerified -= value; }
        }
        List<ResourceVerifiyInfo> verifiyResult = new List<ResourceVerifiyInfo>();
        List<ResourceVerifiyTask> tasks = new List<ResourceVerifiyTask>();

        Coroutine coroutine;
        public void VerifiyManifest(ResourceManifest manifest, string bundlePath)
        {
            tasks.Clear();
            verifiyResult.Clear();
            foreach (var bundleBuildInfo in manifest.ResourceBundleBuildInfoDict.Values)
            {
                var url = Path.Combine(bundlePath, bundleBuildInfo.ResourceBundle.BundleKey);
                tasks.Add(new ResourceVerifiyTask(url, bundleBuildInfo.ResourceBundle.BundleName, bundleBuildInfo.BundleSize));
            }
            coroutine = Utility.Unity.StartCoroutine(MultipleVerifiy(tasks));
        }
        public void StopVerifiy()
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            var length = tasks.Count;
            for (int i = 0; i < length; i++)
            {
                var task = tasks[i];
                verifiyResult.Add(new ResourceVerifiyInfo(task.Url, task.ResourceBundleName, false, false));
            }
            onVerified?.Invoke(verifiyResult.ToArray());
            tasks.Clear();
            verifiyResult.Clear();
        }
        IEnumerator MultipleVerifiy(List<ResourceVerifiyTask> tasks)
        {
            while (tasks.Count > 0)
            {
                var task = tasks[0];
                yield return VerifiyContentLength(task);
            }
            onVerified?.Invoke(verifiyResult.ToArray());
        }
        IEnumerator VerifiyContentLength(ResourceVerifiyTask task)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(task.Url))
            {
                yield return request.SendWebRequest();
                var size = request.GetRequestHeader("Content-Length");
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    var bundleLength = Convert.ToInt64(size);
                    bool isEqual = false;
                    if (task.ResourceBundleSize == bundleLength)
                    {
                        isEqual = true;
                    }
                    verifiyResult.Add(new ResourceVerifiyInfo(task.Url, task.ResourceBundleName, true, isEqual));
                }
                else
                {
                    verifiyResult.Add(new ResourceVerifiyInfo(task.Url, task.ResourceBundleName, false, false));
                }
            }
        }
    }
}
