using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        /// Key : [ABName] ; Value : [QuarkAssetBundle]
        /// </summary>
        Dictionary<string, QuarkAssetBundle> assetBundleDict = new Dictionary<string, QuarkAssetBundle>();
        /// <summary>
        /// BuiltAssetBundle 模式下资源的映射；
        /// Key : AssetName---Value :  Lnk [QuarkAssetABObjectWapper]
        /// </summary>` 
        Dictionary<string, LinkedList<QuarkAssetObjectWapper>> builtAssetBundleMap
            = new Dictionary<string, LinkedList<QuarkAssetObjectWapper>>();

        /// <summary>
        /// Hash===QuarkAssetObjectInfo
        /// </summary>
        Dictionary<int, QuarkAssetObjectInfo> hashQuarkAssetObjectInfoDict = new Dictionary<int, QuarkAssetObjectInfo>();
        /// <summary>
        /// 被加载的场景字典；
        /// SceneName===Scene
        /// </summary>
        Dictionary<string, Scene> loadedSceneDict = new Dictionary<string, Scene>();
        public void SetLoaderData(IQuarkLoaderData loaderData)
        {
            SetBuiltAssetBundleModeData(loaderData as QuarkManifest);
        }
        public T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            AssetBundle assetBundle = null;
            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                assetBundleName = wapper.QuarkAssetObject.AssetBundleName;
                if (string.IsNullOrEmpty(assetBundleName))
                {
                    return null;
                }
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    assetBundle = assetBundleDict[assetBundleName].AssetBundle;
                    asset = assetBundle.LoadAsset<T>(assetName);
                    if (asset != null)
                    {
                        IncrementQuarkAssetObject(wapper);
                    }
                }
                else
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                    assetBundle = AssetBundle.LoadFromFile(abPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                    assetBundleDict[assetBundleName] = new QuarkAssetBundle(assetBundleName, assetBundle);
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
                                    AssetBundle abBin = AssetBundle.LoadFromFile(dependentABPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                                    assetBundleDict[dependentABName] = new QuarkAssetBundle(dependentABName, abBin);
                                }
                                catch (Exception e)
                                {
                                    QuarkUtility.LogError(e);
                                }
                            }
                        }
                        if (assetBundle != null)
                        {
                            asset = assetBundle.LoadAsset<T>(assetName);
                            if (asset != null)
                            {
                                IncrementQuarkAssetObject(wapper);
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
            AssetBundle assetBundle = null;
            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                assetBundleName = wapper.QuarkAssetObject.AssetBundleName;
                if (string.IsNullOrEmpty(assetBundleName))
                {
                    return null;
                }
                if (assetBundleDict.ContainsKey(assetBundleName))
                {
                    assetBundle = assetBundleDict[assetBundleName].AssetBundle;
                    assets = assetBundle.LoadAssetWithSubAssets<T>(assetName);
                    if (assets != null)
                    {
                        IncrementQuarkAssetObject(wapper);
                    }
                }
                else
                {
                    var abPath = Path.Combine(PersistentPath, assetBundleName);
                    assetBundle = AssetBundle.LoadFromFile(abPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                    assetBundleDict[assetBundleName] = new QuarkAssetBundle(assetBundleName, assetBundle);
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
                                    AssetBundle abBin = AssetBundle.LoadFromFile(dependentABPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                                    assetBundleDict[dependentABName] = new QuarkAssetBundle(assetBundleName, abBin);
                                }
                                catch (Exception e)
                                {
                                    QuarkUtility.LogError(e);
                                }
                            }
                        }
                        if (assetBundle != null)
                        {
                            assets = assetBundle.LoadAssetWithSubAssets<T>(assetName);
                            if (assets != null)
                            {
                                IncrementQuarkAssetObject(wapper);
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
        public Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progress, callback, additive));
        }
        public void UnloadAsset(string assetName, string assetExtension)
        {
            var hasWapper = GetAssetDatabaseObject(assetName, assetExtension, out var wapper);
            if (!hasWapper)
            {
                QuarkUtility.LogError($"Asset：{assetName}{assetExtension} not existed !");
                return;
            }
            DecrementQuarkAssetObject(wapper);
        }
        public void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.AssetBundle.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
        }
        public void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assetBundleDict[assetBundleName].AssetBundle.Unload(unloadAllLoadedObjects);
                assetBundleDict.Remove(assetBundleName);
            }
        }
        public Coroutine UnLoadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumUnLoadSceneAsync(sceneName, progress, callback));
        }
        public Coroutine UnLoadAllSceneAsync(Action<float> progress, Action callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumUnLoadAllSceneAsync(progress, callback));
        }
        public QuarkAssetObjectInfo GetInfo<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
                return wapper.GetQuarkAssetObjectInfo();
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo GetInfo(string assetName, string assetExtension)
        {
            var hasWapper = GetAssetDatabaseObject(assetName, assetExtension, out var wapper);
            if (hasWapper)
                return wapper.GetQuarkAssetObjectInfo();
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo[] GetAllLoadedInfos()
        {
            return hashQuarkAssetObjectInfoDict.Values.ToArray();
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            string assetBundleName = string.Empty;

            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                assetBundleName = wapper.QuarkAssetObject.AssetBundleName;
            }
            else
            {
                QuarkUtility.LogError($"Asset：{assetName}{assetExtension} not existed !");
                callback?.Invoke(assets);
                yield break;
            }
            if (string.IsNullOrEmpty(assetBundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assets = assetBundleDict[assetBundleName].AssetBundle.LoadAssetWithSubAssets<T>(assetName);
                if (assets != null)
                {
                    IncrementQuarkAssetObject(wapper);
                }
            }
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
where T : UnityEngine.Object
        {
            T asset = null;
            string assetBundleName = string.Empty;
            var hasWapper = GetAssetDatabaseObject(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                assetBundleName = wapper.QuarkAssetObject.AssetBundleName;
            }
            else
            {
                QuarkUtility.LogError($"Asset：{assetName}{assetExtension} not existed !");
                callback?.Invoke(asset);
                yield break;
            }
            if (string.IsNullOrEmpty(assetBundleName))
                yield break;
            yield return EnumLoadDependenciesAssetBundleAsync(assetBundleName);
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                asset = assetBundleDict[assetBundleName].AssetBundle.LoadAsset<T>(assetName);
                if (asset != null)
                {
                    IncrementQuarkAssetObject(wapper);
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
                    var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                    yield return abReq;
                    var bundle = abReq.assetBundle;
                    if (bundle != null)
                        assetBundleDict[assetBundleName] = new QuarkAssetBundle(assetBundleName, abReq.assetBundle);
                    else
                        QuarkUtility.LogError($"AssetBundle : {assetBundleName} load failure !");
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
                            var abReq = AssetBundle.LoadFromFileAsync(abPath, 0, QuarkDataProxy.QuarkEncryptionOffset);
                            yield return abReq;
                            var bundle = abReq.assetBundle;
                            if (bundle != null)
                                assetBundleDict[dependentABName] = new QuarkAssetBundle(dependentABName, abReq.assetBundle);
                            else
                                QuarkUtility.LogError($"AssetBundle : {dependentABName} load failure !");
                        }
                    }
                }
                yield return null;
            }
        }
        IEnumerator EnumLoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive)
        {
            var hasWapper = GetAssetDatabaseObject(sceneName, ".unity", out var wapper);
            if (hasWapper)
            {
                if (loadedSceneDict.ContainsKey(sceneName))
                {
                    QuarkUtility.LogError($"Scene：{sceneName} is already loaded !");
                    progress?.Invoke(1);
                    callback?.Invoke();
                    yield break;
                }
            }
            else
            {
                QuarkUtility.LogError($"Scene：{sceneName} not existed !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(wapper.QuarkAssetObject.AssetBundleName);
            LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(wapper.QuarkAssetObject.AssetPath, loadSceneMode);
            while (!operation.isDone)
            {
                progress?.Invoke(operation.progress);
                yield return null;
            }
            var scene = SceneManager.GetSceneByPath(wapper.QuarkAssetObject.AssetPath);
            loadedSceneDict.Add(sceneName, scene);
            IncrementQuarkAssetObject(wapper);
            progress?.Invoke(1);
            callback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                QuarkUtility.LogError("Scene name is invalid!");
                yield break;
            }
            var hasWapper = GetAssetDatabaseObject(sceneName, ".unity", out var wapper);
            if (!hasWapper)
            {
                QuarkUtility.LogError($"Scene：{sceneName}.unity not existed !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            else
            {
                QuarkUtility.LogError($"Scene：{sceneName}.unity not existed !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (!loadedSceneDict.TryGetValue(sceneName, out var scene))
            {
                QuarkUtility.LogError($"Unload scene failure： {sceneName}  not loaded yet !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            loadedSceneDict.Remove(sceneName);
            var ao = SceneManager.UnloadSceneAsync(scene);
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                yield return null;
            }
            DecrementQuarkAssetObject(wapper);
            progress?.Invoke(1);
            callback?.Invoke();
        }
        IEnumerator EnumUnLoadAllSceneAsync(Action<float> progress, Action callback)
        {
            var sceneCount = loadedSceneDict.Count;
            //单位场景的百分比比率
            var unitResRatio = 100f / sceneCount;
            int currentSceneIndex = 0;
            float overallProgress = 0;
            foreach (var scene in loadedSceneDict)
            {
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                var sceneName = scene.Key;
                var hasWapper = GetAssetDatabaseObject(sceneName, ".unity", out var wapper);
                if (!hasWapper)
                {
                    overallProgress = overallIndexPercent + (unitResRatio * 1);
                    progress?.Invoke(overallProgress / 100);
                }
                else
                {
                    var ao = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.Value);
                    while (!ao.isDone)
                    {
                        overallProgress = overallIndexPercent + (unitResRatio * ao.progress);
                        progress?.Invoke(overallProgress / 100);
                        yield return null;
                    }
                    overallProgress = overallIndexPercent + (unitResRatio * 1);
                    progress?.Invoke(overallProgress / 100);
                    ReleaseQuarkAssetObject(wapper);
                }
            }
            loadedSceneDict.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        bool GetAssetDatabaseObject<T>(string assetName, string assetExtension, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            var typeString = typeof(T).ToString();
            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    var obj = abLnk.First.Value;
                    if (obj.QuarkAssetObject.AssetType == typeString)
                        wapper = abLnk.First.Value;
                }
                else
                {
                    foreach (var abWapper in abLnk)
                    {
                        if (abWapper.QuarkAssetObject.AssetExtension == assetExtension && abWapper.QuarkAssetObject.AssetType == typeString)
                        {
                            wapper = abWapper;
                            break;
                        }
                    }
                }
            }
            return wapper != null;
        }
        bool GetAssetDatabaseObject(string assetName, string assetExtension, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            if (builtAssetBundleMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    wapper = abLnk.First.Value;
                }
                else
                {
                    foreach (var abWapper in abLnk)
                    {
                        if (abWapper.QuarkAssetObject.AssetExtension == assetExtension)
                        {
                            wapper = abWapper;
                            break;
                        }
                    }
                }
            }
            return wapper != null;
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
                var abName = mf.Value.ABName;
                foreach (var a in assets)
                {
                    var quarkObject = a.Value;
                    if (!builtAssetBundleMap.TryGetValue(quarkObject.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetObjectWapper>();
                        var wapper = new QuarkAssetObjectWapper();
                        wapper.QuarkAssetObject = quarkObject;
                        lnkList.AddLast(wapper);
                        builtAssetBundleMap.Add(quarkObject.AssetName, lnkList);
                    }
                    else
                    {
                        var wapper = new QuarkAssetObjectWapper();
                        wapper.QuarkAssetObject = quarkObject;
                        lnkList.AddLast(wapper);
                    }
                }
            }
        }
        /// <summary>
        /// 增加一个资源对象的引用计数；
        /// </summary>
        void IncrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            wapper.AssetReferenceCount++;
            hashQuarkAssetObjectInfoDict[wapper.GetHashCode()] = wapper.GetQuarkAssetObjectInfo();
            //增加一个AB的引用计数；
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle.ReferenceCount++;
            }
        }
        /// <summary>
        /// 减少一个资源对象的引用计数；
        /// </summary>
        void DecrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            wapper.AssetReferenceCount--;
            if (wapper.AssetReferenceCount <= 0)
            {
                wapper.AssetReferenceCount=0;
            }
            var hashCode = wapper.GetHashCode();
            hashQuarkAssetObjectInfoDict[hashCode] = wapper.GetQuarkAssetObjectInfo();
            //减少一个AB的引用计数
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle.ReferenceCount--;
                if (assetBundle.ReferenceCount == 0)
                {
                    assetBundle.AssetBundle.Unload(true);
                    assetBundleDict.Remove(assetBundle.AssetBundleName);
                }
            }
        }
        void ReleaseQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            var count = wapper.AssetReferenceCount;
            wapper.AssetReferenceCount = 0;
            var hashCode = wapper.GetHashCode();
            hashQuarkAssetObjectInfoDict[hashCode] = wapper.GetQuarkAssetObjectInfo();
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle.ReferenceCount -= count;
                if (assetBundle.ReferenceCount == 0)
                {
                    assetBundle.AssetBundle.Unload(true);
                    assetBundleDict.Remove(assetBundle.AssetBundleName);
                }
            }
        }
    }
}
