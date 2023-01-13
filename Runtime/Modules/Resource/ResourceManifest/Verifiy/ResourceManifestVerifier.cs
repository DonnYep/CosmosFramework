﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using UnityEngine;

namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// 资源文件校验器；
    /// 用于校验bundle文件是否完整；
    /// </summary>
    public class ResourceManifestVerifier
    {
        Action<ResourceManifestVerifierResult> onVerified;
        public event Action<ResourceManifestVerifierResult> OnVerified
        {
            add { onVerified += value; }
            remove { onVerified -= value; }
        }
        List<ResourceManifestVerifiyTask> tasks = new List<ResourceManifestVerifiyTask>();
        List<ResourceManifestVerifiyInfo> verifiySuccessInfos = new List<ResourceManifestVerifiyInfo>();
        List<ResourceManifestVerifiyInfo> verifiyFailureInfos = new List<ResourceManifestVerifiyInfo>();
        Coroutine coroutine;
        /// <summary>
        /// 校验文件清单；
        /// </summary>
        /// <param name="manifest">文件清单</param>
        /// <param name="bundlePath">持久化路径</param>
        public void VerifiyManifest(ResourceManifest manifest, string bundlePath)
        {
            tasks.Clear();
            verifiySuccessInfos.Clear();
            verifiyFailureInfos.Clear();
            foreach (var bundleBuildInfo in manifest.ResourceBundleBuildInfoDict.Values)
            {
                var url = Path.Combine(bundlePath, bundleBuildInfo.ResourceBundle.BundleKey);
                tasks.Add(ResourceManifestVerifiyTask.Create(url, bundleBuildInfo.ResourceBundle.BundleName, bundleBuildInfo.BundleSize));
            }
            coroutine = Utility.Unity.StartCoroutine(MultipleVerifiy());
        }
        /// <summary>
        /// 停止校验
        /// </summary>
        public void StopVerifiy()
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            var length = tasks.Count;
            for (int i = 0; i < length; i++)
            {
                var task = tasks[i];
                verifiyFailureInfos.Add(new ResourceManifestVerifiyInfo(task.Url, task.ResourceBundleName, false));
            }
            var result = new ResourceManifestVerifierResult()
            {
                VerifiyFailureInfos = verifiyFailureInfos.ToArray(),
                VerifiySuccessInfos = verifiySuccessInfos.ToArray()
            };
            onVerified?.Invoke(result);
            tasks.Clear();
            verifiySuccessInfos.Clear();
            verifiyFailureInfos.Clear();
        }
        IEnumerator MultipleVerifiy()
        {
            while (tasks.Count > 0)
            {
                var task = tasks.RemoveFirst();
                yield return VerifiyContentLength(task);
                ResourceManifestVerifiyTask.Release(task);
            }
            var result = new ResourceManifestVerifierResult()
            {
                VerifiyFailureInfos = verifiyFailureInfos.ToArray(),
                VerifiySuccessInfos = verifiySuccessInfos.ToArray()
            };
            onVerified?.Invoke(result);
        }
        IEnumerator VerifiyContentLength(ResourceManifestVerifiyTask task)
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
                    bool bundleLengthMatched = false;
                    if (task.ResourceBundleSize == bundleLength)
                    {
                        bundleLengthMatched = true;
                    }
                    verifiySuccessInfos.Add(new ResourceManifestVerifiyInfo(task.Url, task.ResourceBundleName, bundleLengthMatched));
                }
                else
                {
                    verifiyFailureInfos.Add(new ResourceManifestVerifiyInfo(task.Url, task.ResourceBundleName, false));
                }
            }
        }
    }
}
