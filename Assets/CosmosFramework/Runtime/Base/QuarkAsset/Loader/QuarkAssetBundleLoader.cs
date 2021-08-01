using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.Quark.Loader
{
    public class QuarkAssetBundleLoader : IQuarkAssetLoader
    {
        string PersistentPath { get { return QuarkDataProxy.PersistentPath; } }
        /// <summary>
        /// 存储ab包之间的引用关系；
        /// </summary>
        QuarkBuildInfo QuarkBuildInfo { get { return QuarkDataProxy.QuarkBuildInfo; } }
        /// <summary>
        /// Key : [ABName] ; Value : [AssetBundle]
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>();
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : AssetName---Value :  Lnk [QuarkAssetABObject]
        /// </summary>` 
        Dictionary<string, LinkedList<QuarkAssetBundleObject>> builtAssetBundleMap
            = new Dictionary<string, LinkedList<QuarkAssetBundleObject>>();
        public void SetLoaderData(object customeData)
        {
            SetBuiltAssetBundleModeData(customeData as QuarkManifest);
        }
        public T LoadAsset<T>(string assetName, bool instantiate = false) where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abObject))
            {
                assetBundleName = abObject.First.Value.AssetBundleName;
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                    if (asset != null)
                    {
                        if (instantiate)
                        {
                            asset = GameObject.Instantiate(asset);
                        }
                    }
                }
            }
            return asset;
        }
        public T LoadAsset<T>(string assetName, string assetExtension, bool instantiate = false) where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abObject))
            {
                foreach (var ab in abObject)
                {
                    if (ab.AssetExtension == assetExtension)
                    {
                        assetBundleName = ab.AssetBundleName;
                        break;
                    }
                }
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                    if (asset != null)
                    {
                        if (instantiate)
                        {
                            asset = GameObject.Instantiate(asset);
                        }
                    }
                }
            }
            return asset;
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, string.Empty, callback, instantiate));
        }
        public Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, assetExtension, callback, instantiate));
        }
        public void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
        }
        public void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assetBundleDict[assetBundleName].Unload(unloadAllLoadedObjects);
                assetBundleDict.Remove(assetBundleName);
            }
        }
        IEnumerator EnumDownloadAssetBundle(string uri, Action<AssetBundle> doneCallback, Action<string> failureCallback)
        {
            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    failureCallback?.Invoke(request.error);
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                    doneCallback?.Invoke(bundle);
                }
            }
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false)
where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;

            if (builtAssetBundleMap.TryGetValue(assetName, out var abObject))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    assetBundleName = abObject.First.Value.AssetBundleName;
                }
                else
                {
                    foreach (var obj in abObject)
                    {
                        if (obj.AssetExtension == assetExtension)
                        {
                            assetBundleName = obj.AssetBundleName;
                            break;
                        }
                    }
                }
            }
            else
            {
                Utility.Debug.LogError($"资源中不存在：{assetName}");
                yield break;
            }
            if (string.IsNullOrEmpty(assetBundleName))
            {
                callback?.Invoke(asset);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);

            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                if (asset != null)
                {
                    if (instantiate)
                    {
                        asset = GameObject.Instantiate(asset);
                    }
                }
            }
            if (asset != null)
                callback?.Invoke(asset);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (QuarkBuildInfo != null)
            {
                if (!assetBundleDict.ContainsKey(assetBundleName))
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                    yield return EnumDownloadAssetBundle(abPath, ab =>
                    {
                        assetBundleDict.TryAdd(assetBundleName, ab);
                    }, null);
                }
                QuarkBuildInfo.AssetData buildInfo = null;
                foreach (var item in QuarkBuildInfo.AssetDataMaps)
                {
                    if (item.Value.ABName == assetBundleName)
                    {
                        buildInfo = item.Value;
                        break;
                    }
                }
                if (buildInfo != null)
                {
                    var dependList = buildInfo.DependList;
                    var length = dependList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var dependentABName = dependList[i];
                        if (!assetBundleDict.ContainsKey(dependentABName))
                        {
                            var abPath = Path.Combine(PersistentPath, dependentABName);
                            Utility.Debug.LogInfo($"加载 {assetBundleName} 的依赖AB包：{dependentABName}");
                            yield return EnumDownloadAssetBundle(abPath, ab =>
                            {
                                assetBundleDict.TryAdd(dependentABName, ab);
                            }, null);
                        }
                    }
                }
            }
        }
        QuarkAssetBundleObject GetAssetBundleObject(string abName, string assetPath, string assetName)
        {
            var abObject = new QuarkAssetBundleObject()
            {
                AssetBundleName = abName,
                AssetPath = assetPath,
            };
            var strs = Utility.Text.StringSplit(assetPath, new string[] { "/" });
            var nameWithExt = strs[strs.Length - 1];
            var splits = Utility.Text.StringSplit(nameWithExt, new string[] { "." });
            abObject.AssetExtension = Utility.Text.Combine(".", splits[splits.Length - 1]);
            abObject.AssetName = assetName;
            return abObject;
        }
        /// <summary>
        /// 对Manifest进行编码；
        /// </summary>
        /// <param name="manifest">unityWebRequest获取的Manifest文件对象</param>
        void SetBuiltAssetBundleModeData(QuarkManifest manifest)
        {
            //var lnkDict = new Dictionary<string, LinkedList<QuarkAssetBundleObject>>();
            foreach (var mf in manifest.ManifestDict)
            {
                var assets = mf.Value.Assets;
                if (assets == null)
                    continue;
                foreach (var a in assets)
                {
                    var abObject = GetAssetBundleObject(mf.Value.ABName, a.Key, a.Value);
                    if (!builtAssetBundleMap.TryGetValue(abObject.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetBundleObject>();
                        lnkList.AddLast(abObject);
                        builtAssetBundleMap.Add(abObject.AssetName, lnkList);
                    }
                    else
                    {
                        lnkList.AddLast(abObject);
                    }
                }
            }
        }
    }
}
