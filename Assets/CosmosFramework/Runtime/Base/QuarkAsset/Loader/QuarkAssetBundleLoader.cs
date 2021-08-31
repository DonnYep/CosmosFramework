using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Cosmos;
using Utility = Cosmos.Utility;

namespace Quark.Loader
{
    internal class QuarkAssetBundleLoader : IQuarkAssetLoader
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

        /// <summary>
        /// Hash===QuarkObjectInfo
        /// </summary>
        Dictionary<int, QuarkObjectInfo> hashQuarkObjectInfoDict = new Dictionary<int, QuarkObjectInfo>();

        public void SetLoaderData(object customeData)
        {
            SetBuiltAssetBundleModeData(customeData as QuarkManifest);
        }
        public T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            QuarkAssetBundleObject abObject = null;
            AssetBundle assetBundle = null;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                    assetBundleName = abObject.AssetBundleName;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            assetBundleName = ab.AssetBundleName;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(assetBundleName))
                {
                    return null;
                }
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    assetBundle = assetBundleDict[assetBundleName];
                    asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                    if (asset != null)
                    {
                        IncrementQuarkObjectInfo(abObject);
                    }
                }
                else
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                    assetBundle = AssetBundle.LoadFromFile(abPath);
                    assetBundleDict.TryAdd(assetBundleName, assetBundle);

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
                                var dependentABPath = Path.Combine(PersistentPath, dependentABName);
                                try
                                {
                                    AssetBundle abBin = AssetBundle.LoadFromFile(dependentABPath);
                                    assetBundleDict.TryAdd(dependentABName, abBin);
                                }
                                catch (Exception e)
                                {
                                    Utility.Debug.LogError(e);
                                }
                            }
                        }
                        if (assetBundle != null)
                        {
                            asset = assetBundle.LoadAsset<T>(assetName);
                            if (asset != null)
                            {
                                IncrementQuarkObjectInfo(abObject);
                            }
                        }
                    }
                }
            }
            return asset;
        }
        public GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            var resGo = LoadAsset<GameObject>(assetName, null);
            if (instantiate)
            {
                var go = GameObject.Instantiate(resGo);
                return go;
            }
            else
                return resGo;
        }
        public T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            T[] assets = null;
            string assetBundleName = string.Empty;
            QuarkAssetBundleObject abObject = null;
            AssetBundle assetBundle = null;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                    assetBundleName = abObject.AssetBundleName;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            assetBundleName = ab.AssetBundleName;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(assetBundleName))
                {
                    return null;
                }
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    assetBundle = assetBundleDict[assetBundleName];
                    assets = assetBundleDict[assetBundleName].LoadAssetWithSubAssets<T>(assetName);
                    if (assets != null)
                    {
                        IncrementQuarkObjectInfo(abObject);
                    }
                }
                else
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                    assetBundle = AssetBundle.LoadFromFile(abPath);
                    assetBundleDict.TryAdd(assetBundleName, assetBundle);

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
                                var dependentABPath = Path.Combine(PersistentPath, dependentABName);
                                try
                                {
                                    AssetBundle abBin = AssetBundle.LoadFromFile(dependentABPath);
                                    assetBundleDict.TryAdd(dependentABName, abBin);
                                }
                                catch (Exception e)
                                {
                                    Utility.Debug.LogError(e);
                                }
                            }
                        }
                        if (assetBundle != null)
                        {
                            assets = assetBundle.LoadAssetWithSubAssets<T>(assetName);
                            if (assets != null)
                            {
                                IncrementQuarkObjectInfo(abObject);
                            }
                        }
                    }
                }
            }
            return assets;
        }
        public Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, assetExtension, callback));
        }
        public Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync<GameObject>(assetName, assetExtension, (resGo) =>
            {
                if (instantiate)
                {
                    var go = GameObject.Instantiate(resGo);
                    callback.Invoke(go);
                }
                else
                {
                    callback.Invoke(resGo);
                }
            }));
        }
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, assetExtension, callback));
        }
        public Coroutine LoadScenetAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progress, callback, additive));
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

        public QuarkObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            QuarkAssetBundleObject abObject = null;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            break;
                        }
                    }
                }
                return hashQuarkObjectInfoDict[abObject.GetHashCode()];
            }
            return QuarkObjectInfo.None;
        }
        public QuarkObjectInfo[] GetAllInfos()
        {
            return hashQuarkObjectInfoDict.Values.ToArray();
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            string assetBundleName = string.Empty;
            QuarkAssetBundleObject abObject = null;

            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                    assetBundleName = abObject.AssetBundleName;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            assetBundleName = ab.AssetBundleName;
                            break;
                        }
                    }
                }
            }
            else
            {
                Utility.Debug.LogError($"asset：{assetName} not existed !");
                yield break;
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Utility.Debug.LogError($"AssetBundle：{assetBundleName} not existed !");
                callback?.Invoke(assets);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assets = assetBundleDict[assetBundleName].LoadAssetWithSubAssets<T>(assetName);
                if (assets != null)
                {
                    IncrementQuarkObjectInfo(abObject);
                }
            }
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            QuarkAssetBundleObject abObject = null;

            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                    assetBundleName = abObject.AssetBundleName;
                }
                else
                {
                    foreach (var ab in abLnk)
                    {
                        if (ab.AssetExtension == assetExtension)
                        {
                            abObject = ab;
                            assetBundleName = ab.AssetBundleName;
                            break;
                        }
                    }
                }
            }
            else
            {
                Utility.Debug.LogError($"asset：{assetName} not existed !");
                yield break;
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Utility.Debug.LogError($"AssetBundle：{assetBundleName} not existed !");
                callback?.Invoke(asset);
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].LoadAsset<T>(assetName);
                if (asset != null)
                {
                    IncrementQuarkObjectInfo(abObject);
                }
            }
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (QuarkBuildInfo != null)
            {
                if (!assetBundleDict.ContainsKey(assetBundleName))
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                   // Utility.Debug.LogInfo(abPath);
                    try
                    {
                        AssetBundle abBin = AssetBundle.LoadFromFile(abPath);
                        assetBundleDict.TryAdd(assetBundleName, abBin);
                        //Utility.Debug.LogInfo("LoadAB : " + abPath, MessageColor.DARKBLUE);
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError(e);
                    }
                    //yield return EnumDownloadAssetBundle(abPath, ab =>
                    //{
                    //    Utility.Debug.LogInfo("assetBundleName : " + assetBundleName, MessageColor.NAVY);
                    //    assetBundleDict.TryAdd(assetBundleName, ab);
                    //    Utility.Debug.LogInfo("LoadAB : " + abPath, MessageColor.DARKBLUE);
                    //}, null);
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
                        //Utility.Debug.LogInfo("dependentABName : " + dependentABName, MessageColor.ORANGE);
                        if (!assetBundleDict.ContainsKey(dependentABName))
                        {
                            var abPath = Path.Combine(PersistentPath, dependentABName);

                            try
                            {
                                AssetBundle abBin = AssetBundle.LoadFromFile(abPath);
                                assetBundleDict.TryAdd(dependentABName, abBin);
                                //Utility.Debug.LogInfo("dependentABName : " + dependentABName, MessageColor.FUCHSIA);
                            }
                            catch (Exception e)
                            {
                                Utility.Debug.LogError(e);
                            }
                            //yield return EnumDownloadAssetBundle(abPath, ab =>
                            //{
                            //    Utility.Debug.LogInfo("dependentABName : " + dependentABName, MessageColor.FUCHSIA);
                            //    assetBundleDict.TryAdd(dependentABName, ab);
                            //}, (str) => { Utility.Debug.LogError(str); });
                        }
                    }
                }
                yield return null;
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
                yield return null;
            }
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, Action<float> loadingCallback, Action callback, bool additive)
        {
            string assetBundleName = string.Empty;
            if (builtAssetBundleMap.TryGetValue(sceneName, out var abObject))
            {
                assetBundleName = abObject.First.Value.AssetBundleName;
            }
            else
            {
                Utility.Debug.LogError($"Scene：{sceneName} not existed !");
                yield break;
            }
            if (string.IsNullOrEmpty(assetBundleName))
            {
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            while (!operation.isDone)
            {
                loadingCallback?.Invoke(operation.progress);
                yield return null;
            }
            loadingCallback?.Invoke(1);
            callback?.Invoke();
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
                    var info = QuarkObjectInfo.Create(abObject.AssetName, abObject.AssetBundleName, abObject.AssetExtension, 0);
                    info.ABObjectHash = abObject.GetHashCode();
                    hashQuarkObjectInfoDict.Add(info.ABObjectHash, info);
                }
            }
        }
        /// <summary>
        /// 增加一个引用计数；
        /// </summary>
        void IncrementQuarkObjectInfo(QuarkAssetBundleObject abObject)
        {
            var hashCode = abObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info++;
        }
        /// <summary>
        /// 减少一个引用计数；
        /// </summary>
        void DecrementQuarkObjectInfo(QuarkAssetBundleObject abObject)
        {
            var hashCode = abObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info--;
        }


    }
}
