﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using UnityEngine;

namespace Cosmos.Resource.Verify
{
    /// <summary>
    /// 资源文件校验器；
    /// 用于校验bundle文件是否完整；
    /// </summary>
    public class ResourceManifestVerifier
    {
        Action<ResourceManifestVerifyResult> onVerified;
        public event Action<ResourceManifestVerifyResult> OnVerified
        {
            add { onVerified += value; }
            remove { onVerified -= value; }
        }
        List<ResourceManifestVerifyTask> tasks = new List<ResourceManifestVerifyTask>();
        List<ResourceManifestVerifyInfo> verificationSuccessInfos = new List<ResourceManifestVerifyInfo>();
        List<ResourceManifestVerifyInfo> verificationFailureInfos = new List<ResourceManifestVerifyInfo>();
        Coroutine coroutine;
        bool verificationInProgress;
        public bool VerificationInProgress
        {
            get { return verificationInProgress; }
        }
        /// <summary>
        /// 校验文件清单；
        /// </summary>
        /// <param name="manifest">文件清单</param>
        /// <param name="bundlePath">持久化路径</param>
        public void VerifyManifest(ResourceManifest manifest, string bundlePath)
        {
            if (verificationInProgress)
                return;
            verificationInProgress = true;
            tasks.Clear();
            verificationSuccessInfos.Clear();
            verificationFailureInfos.Clear();
            foreach (var bundleBuildInfo in manifest.ResourceBundleBuildInfoDict.Values)
            {
                var url = Path.Combine(bundlePath, bundleBuildInfo.ResourceBundle.BundleKey);
                tasks.Add(ResourceManifestVerifyTask.Create(url, bundleBuildInfo.ResourceBundle.BundleName, bundleBuildInfo.BundleSize));
            }
            coroutine = Utility.Unity.StartCoroutine(MultipleVerify());
        }
        /// <summary>
        /// 停止校验；
        /// </summary>
        public void StopVerify()
        {
            if (coroutine != null)
                Utility.Unity.StopCoroutine(coroutine);
            var length = tasks.Count;
            for (int i = 0; i < length; i++)
            {
                var task = tasks[i];
                verificationFailureInfos.Add(new ResourceManifestVerifyInfo(task.Url, task.ResourceBundleName, false, 0));
            }
            var result = new ResourceManifestVerifyResult()
            {
                VerificationFailureInfos = verificationFailureInfos.ToArray(),
                VerificationSuccessInfos = verificationSuccessInfos.ToArray()
            };
            verificationInProgress = false;
            onVerified?.Invoke(result);
            tasks.Clear();
            verificationSuccessInfos.Clear();
            verificationFailureInfos.Clear();
        }
        IEnumerator MultipleVerify()
        {
            while (tasks.Count > 0)
            {
                var task = tasks.RemoveFirst();
                yield return VerifyContentLength(task);
                ResourceManifestVerifyTask.Release(task);
            }
            var result = new ResourceManifestVerifyResult()
            {
                VerificationFailureInfos = verificationFailureInfos.ToArray(),
                VerificationSuccessInfos = verificationSuccessInfos.ToArray()
            };
            verificationInProgress = false;
            onVerified?.Invoke(result);
        }
        IEnumerator VerifyContentLength(ResourceManifestVerifyTask task)
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
                    verificationSuccessInfos.Add(new ResourceManifestVerifyInfo(task.Url, task.ResourceBundleName, bundleLengthMatched, bundleLength));
                }
                else
                {
                    verificationFailureInfos.Add(new ResourceManifestVerifyInfo(task.Url, task.ResourceBundleName, false, 0));
                }
            }
        }
    }
}
