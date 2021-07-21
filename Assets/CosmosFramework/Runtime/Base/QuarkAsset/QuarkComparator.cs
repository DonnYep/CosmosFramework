using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Cosmos.Quark
{
    /// <summary>
    /// Quark Manifest比较器；
    /// </summary>
    public class QuarkComparator
    {
        /// <summary>
        /// 比较失败，传入ErrorMessage；
        /// </summary>
        Action<string> onCompareFailure;
        /// <summary>
        /// Latest===Expired==OverallSize(需要下载的大小)
        /// </summary>
        Action<string[], string[], long> onCompareSuccess;
        //名字相同，但是HASH不同，则认为资源有作修改，需要加入到最新队列中；
        List<string> latest = new List<string>();
        //本地有但是远程没有，则标记为可过期文件；
        List<string> expired = new List<string>();
        /// <summary>
        /// 本地持久化路径；
        /// </summary>
        public string LocalPath { get; private set; }
        /// <summary>
        /// 远程资源地址；
        /// </summary>
        public string URL { get; private set; }
        QuarkABBuildInfo buildInfo;
        QuarkManifest localManifest = null;
        QuarkManifest remoteManifest = null;
        public QuarkComparator(string url, string localPath)
        {
            URL = url;
            LocalPath = localPath;
        }
        public void Initiate(Action<string[], string[], long> onCompareSuccess, Action<string> onCompareFailure)
        {
            this.onCompareSuccess = onCompareSuccess;
            this.onCompareFailure = onCompareFailure;
        }
        public void LoadBuildInfo()
        {
            var buildInfoUrl = Utility.IO.WebPathCombine(LocalPath, QuarkConsts.BuildInfoFileName);
            QuarkUtility.Unity.DownloadTextAsync(buildInfoUrl, null, json =>
            {
                buildInfo = Utility.Json.ToObject<QuarkABBuildInfo>(json);
                //Utility.Debug.LogInfo("LoadBuildInfo Done");
            }, null);
        }
        /// <summary>
        /// 比较remote与local的manifest文件；
        /// </summary>
        public void CompareLocalAndRemoteManifest()
        {
            var uriManifestPath = Utility.IO.WebPathCombine(URL, QuarkConsts.ManifestName);
            QuarkUtility.Unity.StartCoroutine(EnumDownloadManifest(uriManifestPath));
        }
        public void Clear()
        {
            latest.Clear();
            expired.Clear();
            LocalPath = string.Empty;
            URL = string.Empty;
            onCompareSuccess = null;
            onCompareFailure = null;
        }
        IEnumerator EnumDownloadManifest(string uri)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        OnUriManifestSuccess(request.downloadHandler.text);
                    }
                }
                else
                {
                    OnUriManifestFailure(request.error);
                }
            }
        }
        void OnUriManifestFailure(string errorMessage)
        {
            onCompareFailure?.Invoke(errorMessage);
        }
        void OnUriManifestSuccess(string manifestContext)
        {

            var localManifestPath = Utility.IO.WebPathCombine(LocalPath, QuarkConsts.ManifestName);
            string localManifestContext = string.Empty;
            try
            {
                localManifestContext = Utility.IO.ReadTextFileContent(localManifestPath);
            }
            catch { }
            try { localManifest = Utility.Json.ToObject<QuarkManifest>(localManifestContext); }
            catch { }
            try { remoteManifest = Utility.Json.ToObject<QuarkManifest>(manifestContext); }
            catch { }
            if (localManifest != null)
            {
                //若本地的Manifest不为空，远端的Manifest不为空，则对比二者之间的差异；
                //远端有本地没有，则缓存至latest；
                //远端没有本地有，则缓存至expired；
                if (remoteManifest != null)
                {
                    foreach (var remoteMF in remoteManifest.ManifestDict)
                    {
                        if (localManifest.ManifestDict.TryGetValue(remoteMF.Key, out var localMF))
                        {
                            if (localMF.Hash != remoteMF.Value.Hash)
                            {
                                latest.Add(remoteMF.Value.ABName);
                            }
                        }
                        else
                        {
                            latest.Add(remoteMF.Value.ABName);
                        }
                    }
                    foreach (var localMF in localManifest.ManifestDict)
                    {
                        if (!remoteManifest.ManifestDict.ContainsKey(localMF.Key))
                        {
                            expired.Add(localMF.Key);
                        }
                    }
                }
            }
            else
            {
                //若本地的Manifest为空，远端的Manifest不为空，则将需要下载的资源url缓存到latest;
                if (remoteManifest != null)
                {
                    latest.AddRange(remoteManifest.ManifestDict.Keys.ToList());
                }
            }
            var latesetArray = latest.ToArray();
            var expiredArray = expired.ToArray();
            latest.Clear();
            expired.Clear();

            QuarkUtility.Unity.StartCoroutine(EnumGetMultiFilesSize(latesetArray, size =>
            {
                onCompareSuccess?.Invoke(latesetArray, expiredArray, size);
            }));

            var localNewManifestPath = Utility.IO.PathCombine(LocalPath, QuarkConsts.ManifestName);
            Utility.IO.OverwriteTextFile(localNewManifestPath, manifestContext);
        }
        IEnumerator EnumGetMultiFilesSize(string[] uris, Action<long> callback)
        {
            long overallSize = 0;
            var length = uris.Length;
            for (int i = 0; i < length; i++)
            {
                var uri = Path.Combine(URL, uris[i]);
                yield return EnumGetFileSize(uri, size =>
                {
                    if (size >= 0)
                        overallSize += size;
                });
            }
            callback?.Invoke(overallSize);
        }
        IEnumerator EnumGetFileSize(string uri, Action<long> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(uri))
            {
                yield return request.SendWebRequest();
                string size = request.GetResponseHeader("Content-Length");
                if (request.isNetworkError || request.isHttpError)
                    callback?.Invoke(-1);
                else
                    callback?.Invoke(Convert.ToInt64(size));
            }
        }
    }
}
