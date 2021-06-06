using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;

namespace Cosmos.Quark
{
    public class QuarkManager : Singleton<QuarkManager>
    {
        QuarkManifest quarkManifest;
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : ABName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>` 
        Dictionary<string, LinkedList<QuarkAssetBundleObject>> builtAssetBundleMap;
        /// <summary>
        /// Key:[ABName] ; Value : [ABHash]
        /// </summary>
        Dictionary<string, string> assetBundleHashDict;
        /// <summary>
        /// Key : [ABName] ; Value : [AssetBundle]
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict;
        readonly string manifestFileName = "Manifest.json";
        readonly string buildInfoFileName = "BuildInfo.json";

        public QuarkAssetLoadMode QuarkAssetLoadMode { get; set; }

        /// <summary>
        /// 本地缓存的地址；
        /// </summary>
        public string LocalUrl { get; set; }
        /// <summary>
        /// 远端存储的地址；
        /// </summary>
        public string RemoteUrl { get; set; }

        //名字相同，但是HASH不同，则认为资源有作修改，需要加入到下载队列中；
        List<string> downloadable = new List<string>();
        //本地有但是远程没有，则标记为可删除的文件，并加入到可删除队列；
        List<string> deletable = new List<string>();

        #region BuiltAssetBundle
        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        public void SetBuiltAssetBundleModeData(QuarkManifest manifest)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetBundleObject>>();
            foreach (var mf in manifest.ManifestDict)
            {
                var assetPaths = mf.Value.Assets;
                var length = assetPaths.Length;
                for (int i = 0; i < length; i++)
                {
                    var qab = GetAssetBundleObject(mf.Value.ABName, assetPaths[i]);
                    if (!lnkDict.TryGetValue(qab.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetBundleObject>();
                        lnkList.AddLast(qab);
                        lnkDict.Add(qab.AssetName, lnkList);
                    }
                    else
                    {
                        lnkList.AddLast(qab);
                    }
                }
            }
            builtAssetBundleMap = lnkDict;
        }
        #endregion
        public T[] LoadAllAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            T[] asset = null;
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAllAssets<T>();
                if (asset == null)
                {
                    throw new ArgumentNullException($"AB包 {assetBundleName} 中不存在资源 {assetName} ！");
                }
            }
            return asset;
        }
        public T LoadAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            T asset = null;
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                if (asset == null)
                {
                    throw new ArgumentNullException($"AB包 {assetBundleName} 中不存在资源 {assetName} ！");
                }
            }
            return asset;
        }
        /// <summary>
        /// 检测 local与Remote之间manifest的差异；
        /// 若存在差异，回调中传入ture；
        /// 若不存在差异，回调中传入false；
        /// </summary>
        /// <param name="updatableCallback">是否可更新回调</param>
        public Coroutine CheckLatestManifestAsync(Action<bool> updatableCallback)
        {
            return Utility.Unity.StartCoroutine(EnumCheckLatestManifest(updatableCallback));
        }
        /// <summary>
        /// 异步下载AB资源；
        /// </summary>
        /// <param name="overallProgress">下载的整体进度</param>
        /// <param name="progress">单个资源下载的进度</param>
        /// <returns>协程对象</returns>
        public Coroutine DownloadAssetBundlesAsync(Action<float> overallProgress, Action<float> progress)
        {
            return Utility.Unity.StartCoroutine(EnumDownloadAssetBundle(overallProgress, progress));
        }
        static QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetName)
        {
            var qab = new QuarkAssetBundleObject()
            {
                AssetBundleName = abName,
                AssetPath = assetName,
            };
            var strs = Utility.Text.StringSplit(assetName, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            qab.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            qab.AssetName = nameWithExt.Replace(qab.AssetExtension, "");
            return qab;
        }
        IEnumerator EnumCheckLatestManifest(Action<bool> updatableCallback)
        {
            QuarkManifest localManifest = null;
            QuarkManifest remoteManifest = null;
            if (!string.IsNullOrEmpty(LocalUrl))
            {
                var localManifestUrl = Utility.IO.WebPathCombine(LocalUrl, manifestFileName);
                yield return Utility.Unity.DownloadTextAsync(localManifestUrl, null, manifestText =>
                 {
                     try
                     {
                         localManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                     }
                     catch { }
                 });
            }
            else
                throw new ArgumentNullException("LocalAssetBundleUrl is invalid ! ");
            if (!string.IsNullOrEmpty(RemoteUrl))
            {
                var remoteManifestUrl = Utility.IO.WebPathCombine(RemoteUrl, manifestFileName);
                yield return Utility.Unity.DownloadTextAsync(remoteManifestUrl, null, manifestText =>
                 {
                     try
                     {
                         remoteManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                     }
                     catch { }
                 });
            }
            else
                throw new ArgumentNullException("RemoteManifestUrl is invalid ! ");

            if (localManifest != null)
            {
                //若本地的Manifest不为空，远端的Manifest不为空，则对比二者之间的差异；
                //远端有本地没有，则缓存至downloadable；
                //远端没有本地有，则缓存至deleteable；
                if (remoteManifest != null)
                {
                    foreach (var remoteMF in remoteManifest.ManifestDict)
                    {
                        if (localManifest.ManifestDict.TryGetValue(remoteMF.Key, out var localMF))
                        {
                            if (localMF.Hash != remoteMF.Value.Hash)
                            {
                                downloadable.Add(remoteMF.Value.ABName);
                            }
                        }
                        else
                        {
                            downloadable.Add(remoteMF.Value.ABName);
                        }
                    }
                    foreach (var localMF in localManifest.ManifestDict)
                    {
                        if (!remoteManifest.ManifestDict.ContainsKey(localMF.Key))
                        {
                            deletable.Add(localMF.Key);
                        }
                    }
                }
            }
            else
            {
                //若本地的Manifest为空，远端的Manifest不为空，则将需要下载的资源url缓存到downloadable;
                if (remoteManifest != null)
                {
                    downloadable.AddRange(remoteManifest.ManifestDict.Keys.ToList());
                }
            }
            downloadable.TrimExcess();
            deletable.TrimExcess();
            if (downloadable.Count > 0 || deletable.Count > 0)
                updatableCallback.Invoke(true);
            else
                updatableCallback.Invoke(false);
        }
        IEnumerator EnumDownloadAssetBundle(Action<float> overallProgress, Action<float> progress)
        {
            var downloadableAssetUrls = new string[downloadable.Count];
            var downloadLength = downloadableAssetUrls.Length;
            for (int i = 0; i < downloadLength; i++)
            {
                downloadableAssetUrls[i] = Utility.IO.WebPathCombine(RemoteUrl, downloadable[i]);
            }
            //删除本地多余的资源；
            var deleteLength = deletable.Count;
            for (int i = 0; i < deleteLength; i++)
            {
                var deleteFilePath = Utility.IO.WebPathCombine(LocalUrl, deletable[i]);
                Utility.IO.DeleteFile(deleteFilePath);
            }
            yield return Utility.Unity.DownloadAssetBundlesBytesAsync(downloadableAssetUrls, overallProgress, progress, bundleBytes =>
            {
                var bundleLength = bundleBytes.Count;
                for (int i = 0; i < bundleLength; i++)
                {
                    var cachePath = Utility.IO.WebPathCombine(LocalUrl, downloadable[i]);
                    Utility.IO.WriteFile(bundleBytes[i], cachePath);
                }
            });
        }
        T LoadAssetFromABByName<T>(string assetPath, string assetExtension = null)
where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetBundleObject abObject = QuarkAssetBundleObject.None;
            if (builtAssetBundleMap.TryGetValue(assetPath, out var lnkList))
            {
                if (!string.IsNullOrEmpty(assetExtension))
                {
                    abObject = lnkList.First.Value;
                }
                else
                {
                    foreach (var obj in lnkList)
                    {
                        if (obj.AssetExtension == assetExtension)
                        {
                            abObject = obj;
                            break;
                        }
                    }
                }
                //TODO Get AssetBundle
                // var assetBundle = LoadAssetBundle(abObject);
            }
            return null;
        }
        IEnumerator EnumLoadAssetBundleAsync(QuarkAssetBundleObject qabObject)
        {
            if (!assetBundleDict.TryGetValue(qabObject.AssetBundleName, out var assetBundle))
            {
                var url = Utility.IO.WebPathCombine(LocalUrl, qabObject.AssetBundleName);
                yield return Utility.Unity.DownloadAssetBundleAsync(url, null, ab =>
                {
                    assetBundleDict.TryAdd(qabObject.AssetBundleName, ab);
                    assetBundle = ab;
                });
            }
        }
    }
}
