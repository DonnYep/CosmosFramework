using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quark.Loader
{
    internal class QuarkAssetBundleLoader : QuarkAssetLoader
    {
        string PersistentPath { get { return QuarkDataProxy.PersistentPath; } }
        /// <summary>
        /// 存储ab包之间的引用关系；
        /// </summary>
        QuarkBuildInfo QuarkBuildInfo { get { return QuarkDataProxy.QuarkBuildInfo; } }
        public override void SetLoaderData(IQuarkLoaderData loaderData)
        {
            SetBuiltAssetBundleModeData(loaderData as QuarkManifest);
        }
        public override T LoadAsset<T>(string assetName, string assetExtension)
        {
            T asset = null;
            string assetBundleName = string.Empty;
            AssetBundle assetBundle = null;
            var hasWapper = GetAssetObjectWapper<T>(assetName, assetExtension, out var wapper);
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
        public override GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            var resGo = LoadAsset<GameObject>(assetName, assetExtension);
            if (instantiate)
            {
                var go = GameObject.Instantiate(resGo);
                return go;
            }
            else
                return resGo;
        }
        public override T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension)
        {
            T[] assets = null;
            string assetBundleName = string.Empty;
            AssetBundle assetBundle = null;
            var hasWapper = GetAssetObjectWapper<T>(assetName, assetExtension, out var wapper);
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
        public override Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetAsync(assetName, assetExtension, callback));
        }
        public override Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
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
        public override Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, assetExtension, callback));
        }
        public override Coroutine LoadSceneAsync(string sceneName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progressProvider, progress, condition, callback, additive));
        }
        public override void UnloadAsset(string assetName, string assetExtension)
        {
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, out var wapper);
            if (!hasWapper)
            {
                QuarkUtility.LogError($"Asset：{assetName}{assetExtension} not existed !");
                return;
            }
            DecrementQuarkAssetObject(wapper);
        }
        public override void UnloadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            foreach (var lnk in quarkAssetObjectDict.Values)
            {
                foreach (var wapper in lnk)
                {
                    wapper.AssetReferenceCount = 0;
                }
            }
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.AssetBundle.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);

        }
        public override void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                var bundle = assetBundleDict[assetBundleName];
                foreach (var a in bundle.Assets)
                {
                    a.AssetReferenceCount = 0;
                }
                bundle.AssetBundle.Unload(unloadAllLoadedObjects);
                assetBundleDict.Remove(assetBundleName);
            }
        }
        public override Coroutine UnloadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumUnloadSceneAsync(sceneName, progress, callback));
        }
        public override Coroutine UnloadAllSceneAsync(Action<float> progress, Action callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumUnloadAllSceneAsync(progress, callback));
        }
        /// <summary>
        /// 增加一个资源对象的引用计数；
        /// </summary>
        protected override void IncrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            wapper.AssetReferenceCount++;
            hashQuarkAssetObjectInfoDict[wapper.GetHashCode()] = wapper.GetQuarkAssetObjectInfo();
            //增加一个AB的引用计数；
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle.ReferenceCount++;
                assetBundle.Assets.Add(wapper);
            }
        }
        /// <summary>
        /// 减少一个资源对象的引用计数；
        /// </summary>
        protected override void DecrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            wapper.AssetReferenceCount--;
            var hashCode = wapper.GetHashCode();
            hashQuarkAssetObjectInfoDict[hashCode] = wapper.GetQuarkAssetObjectInfo();
            //减少一个AB的引用计数
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                if (wapper.AssetReferenceCount <= 0)
                {
                    wapper.AssetReferenceCount = 0;
                    assetBundle.Assets.Remove(wapper);
                }
                assetBundle.ReferenceCount--;
                if (assetBundle.ReferenceCount == 0)
                {
                    assetBundle.AssetBundle.Unload(true);
                    assetBundleDict.Remove(assetBundle.AssetBundleName);
                }
            }
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
            where T : UnityEngine.Object
        {
            T[] assets = null;
            string assetBundleName = string.Empty;
            var hasWapper = GetAssetObjectWapper<T>(assetName, assetExtension, out var wapper);
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
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, out var wapper);
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
        IEnumerator EnumLoadSceneAsync(string sceneName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback, bool additive)
        {
            if (loadedSceneDict.ContainsKey(sceneName))
            {
                QuarkUtility.LogError($"Scene：{sceneName} is already loaded !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var hasWapper = GetAssetObjectWapper(sceneName, ".unity", out var wapper);
            if (!hasWapper)
            {
                QuarkUtility.LogError($"Scene：{sceneName} not existed !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            yield return EnumLoadDependenciesAssetBundleAsync(wapper.QuarkAssetObject.AssetBundleName);
            LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
            var operation = SceneManager.LoadSceneAsync(wapper.QuarkAssetObject.AssetPath, loadSceneMode);
            operation.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!operation.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + operation.progress;
                    if (sum >= 1.9)
                    {
                        break;
                    }
                    else
                    {
                        progress?.Invoke(sum / 2);
                    }
                }
                else
                {
                    progress?.Invoke(operation.progress);
                    if (operation.progress >= 0.9f)
                    {
                        break;
                    }
                }
                yield return null;
            }
            progress?.Invoke(1);
            if (condition != null)
                yield return new WaitUntil(condition);
            var scene = SceneManager.GetSceneByPath(wapper.QuarkAssetObject.AssetPath);
            loadedSceneDict.Add(sceneName, scene);
            IncrementQuarkAssetObject(wapper);
            operation.allowSceneActivation = true;
            callback?.Invoke();
        }
        IEnumerator EnumUnloadAllSceneAsync(Action<float> progress, Action callback)
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
                var hasWapper = GetAssetObjectWapper(sceneName, ".unity", out var wapper);
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
                    if (!quarkAssetObjectDict.TryGetValue(quarkObject.AssetName, out var lnkList))
                    {
                        lnkList = new LinkedList<QuarkAssetObjectWapper>();
                        var wapper = new QuarkAssetObjectWapper();
                        wapper.QuarkAssetObject = quarkObject;
                        lnkList.AddLast(wapper);
                        quarkAssetObjectDict.Add(quarkObject.AssetName, lnkList);
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
