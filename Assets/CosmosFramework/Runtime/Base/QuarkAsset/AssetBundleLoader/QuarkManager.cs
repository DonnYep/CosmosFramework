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
        string localAssetBundleUrl;
        public string LocalAssetBundleUrl { get { return localAssetBundleUrl; } set { localAssetBundleUrl = value; } }

        string remoteAssetBundleUrl;
        public string RemoteAssetBundleUrl { get { return remoteAssetBundleUrl; } set { remoteAssetBundleUrl = value; } }

        //名字相同，但是HASH不同，则认为资源有作修改，需要加入到下载队列中；
        List<string> downloadable = new  List<string>();
        //本地有但是远程没有，则标记为可删除的文件，并加入到可删除队列；
        List <string> deletable = new List<string>();


        public void SetBuiltAssetBundleMap(Dictionary<string, LinkedList<QuarkAssetBundleObject>> lnkDict)
        {
            builtAssetBundleMap = lnkDict;
        }
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

        IEnumerator CheckLatestManifest()
        {
            QuarkManifest localManifest = null;
            QuarkManifest remoteManifest = null;
            if (!string.IsNullOrEmpty(localAssetBundleUrl))
            {
                var localManifestUrl = Utility.IO.WebPathCombine(localAssetBundleUrl, manifestFileName);
                yield return Utility.Unity.DownloadTextAsync(localManifestUrl, null, manifestText =>
                 {
                     localManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                 });
            }
            else
                throw new ArgumentNullException("LocalAssetBundleUrl is invalid ! ");
            if (!string.IsNullOrEmpty(remoteAssetBundleUrl))
            {
                var remoteManifestUrl = Utility.IO.WebPathCombine(remoteAssetBundleUrl, manifestFileName);
                yield return Utility.Unity.DownloadTextAsync(remoteManifestUrl, null, manifestText =>
                 {
                     remoteManifest = Utility.Json.ToObject<QuarkManifest>(manifestText);
                 });
            }
            else
                throw new ArgumentNullException("RemoteManifestUrl is invalid ! ");
            foreach (var remoteMF in remoteManifest.ManifestDict)
            {
                if(localManifest.ManifestDict.TryGetValue(remoteMF.Key,out var localMF))
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
            downloadable.TrimExcess();
            deletable.TrimExcess();
        }
        IEnumerator DownloadAssetBundle()
        {
            var downloadableUrl = new string[downloadable.Count];
            var length = downloadableUrl.Length;
            for (int i = 0; i < length; i++)
            {
                downloadableUrl[i] = Utility.IO.WebPathCombine(remoteAssetBundleUrl, downloadable[i]);
            }
            yield return Utility.Unity.DownloadAssetBundlesAsync(downloadableUrl, null, null, assetBundles => 
            { 
                //TODO
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
        IEnumerator LoadAssetBundleAsync(QuarkAssetBundleObject qabObject)
        {
            if (!assetBundleDict.TryGetValue(qabObject.AssetBundleName, out var assetBundle))
            {
                var url = Utility.IO.WebPathCombine(localAssetBundleUrl, qabObject.AssetBundleName);
                yield return Utility.Unity.DownloadAssetBundleAsync(url, null, ab =>
                {
                    assetBundleDict.TryAdd(qabObject.AssetBundleName, ab);
                    assetBundle = ab;
                });
            }
        }
        //IEnumerator LoadManifest()
        //{

        //}
    }
}
