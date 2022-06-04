using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace Quark.Loader
{
    internal class QuarkAssetDatabaseLoader : QuarkAssetLoader
    {
        public override void SetLoaderData(IQuarkLoaderData loaderData)
        {
            SetAssetDatabaseModeData(loaderData as QuarkAssetDataset);
        }
        public override T LoadAsset<T>(string assetName, string assetExtension)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (quarkAssetObjectDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, typeof(T), out var wapper);
            if (hasWapper)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(wapper.QuarkAssetObject.AssetPath);
                IncrementQuarkAssetObject(wapper);
                return asset;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public override Object LoadAsset(string assetName, string assetExtension, Type type)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (quarkAssetObjectDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, type, out var wapper);
            if (hasWapper)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(wapper.QuarkAssetObject.AssetPath, type);
                IncrementQuarkAssetObject(wapper);
                return asset;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public override GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            var resGGo = LoadAsset<GameObject>(assetName, assetExtension);
            if (instantiate)
                return GameObject.Instantiate(resGGo);
            else
                return resGGo;
        }
        public override T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (quarkAssetObjectDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, typeof(T), out var wapper);
            if (hasWapper)
            {
                var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(wapper.QuarkAssetObject.AssetPath);
                var length = assetObj.Length;
                T[] assets = new T[length];
                for (int i = 0; i < length; i++)
                {
                    assets[i] = assetObj[i] as T;
                }
                IncrementQuarkAssetObject(wapper);
                return assets;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public override Object[] LoadAssetWithSubAssets(string assetName, string assetExtension, Type type)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (quarkAssetObjectDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, type, out var wapper);
            if (hasWapper)
            {
                var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(wapper.QuarkAssetObject.AssetPath);
                var length = assetObj.Length;
                Object[] assets = new Object[length];
                for (int i = 0; i < length; i++)
                {
                    assets[i] = assetObj[i];
                }
                IncrementQuarkAssetObject(wapper);
                return assets;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public override Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadPrefabAsync(assetName, assetExtension, callback, instantiate));
        }
        public override Coroutine LoadSceneAsync(string sceneName, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progressProvider, progress, condition, callback, additive));
        }
        public override Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, assetExtension, callback));
        }
        public override Coroutine LoadAssetWithSubAssetsAsync(string assetName, string assetExtension, Type type, Action<Object[]> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, assetExtension, type, callback));
        }
        public override Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, assetExtension, callback));
        }
        public override Coroutine LoadAssetAsync(string assetName, string assetExtension, Type type, Action<Object> callback)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, assetExtension, type, callback));
        }
        public override void UnloadAsset(string assetName, string assetExtension)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (quarkAssetObjectDict == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetObjectWapper(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                DecrementQuarkAssetObject(wapper);
            }
        }
        public override void UnloadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            QuarkUtility.LogInfo("AssetDatabase Mode UnLoadAllAsset");
            foreach (var lnk in quarkAssetObjectDict.Values)
            {
                foreach (var obj in lnk)
                {
                    obj.AssetReferenceCount = 0;
                }
            }
            assetBundleDict.Clear();
        }
        public override void UnloadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (assetBundleDict.TryGetValue(assetBundleName, out var bundle))
            {
                foreach (var a in bundle.Assets)
                {
                    a.AssetReferenceCount = 0;
                }
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
        /// <inheritdoc/>
        protected override void IncrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            wapper.AssetReferenceCount++;
            hashQuarkAssetObjectInfoDict[wapper.GetHashCode()] = wapper.GetQuarkAssetObjectInfo();
            //增加一个AB的引用计数；
            if (!assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle = new QuarkAssetBundle(wapper.QuarkAssetObject.AssetBundleName, null);
                assetBundleDict.Add(assetBundle.AssetBundleName, assetBundle);
            }
            assetBundle.ReferenceCount++;
        }
        /// <inheritdoc/>
        protected override void DecrementQuarkAssetObject(QuarkAssetObjectWapper wapper)
        {
            var refCount = wapper.AssetReferenceCount;
            if (refCount <= 0)
            {
                QuarkUtility.LogError($"{wapper.QuarkAssetObject.AssetName}{wapper.QuarkAssetObject.AssetExtension} not loaded !");
                return;
            }
            wapper.AssetReferenceCount--;
            hashQuarkAssetObjectInfoDict[wapper.GetHashCode()] = wapper.GetQuarkAssetObjectInfo();
            //减少一个AB的引用计数
            if (assetBundleDict.TryGetValue(wapper.QuarkAssetObject.AssetBundleName, out var assetBundle))
            {
                assetBundle.ReferenceCount--;
                if (assetBundle.ReferenceCount == 0)
                {
                    assetBundleDict.Remove(assetBundle.AssetBundleName);
                }
            }
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
    where T : UnityEngine.Object
        {
            var assets = LoadAssetWithSubAssets<T>(assetName, assetExtension);
            yield return null;
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync(string assetName, string assetExtension, Type type, Action<Object[]> callback)
        {
            var assets = LoadAssetWithSubAssets(assetName, assetExtension, type);
            yield return null;
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : Object
        {
            var asset = LoadAsset<T>(assetName, assetExtension);
            yield return null;
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadAsssetAsync(string assetName, string assetExtension, Type type, Action<Object> callback)
        {
            var asset = LoadAsset(assetName, assetExtension, type);
            yield return null;
            callback?.Invoke(asset);
        }
        IEnumerator EnumLoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate)
        {
            var resGo = LoadAsset<GameObject>(assetName, assetExtension);
            yield return null;
            if (instantiate)
            {
                var go = GameObject.Instantiate(resGo);
                callback?.Invoke(go);
            }
            else
            {
                callback?.Invoke(resGo);
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
            LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
#if UNITY_EDITOR
            var operation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(wapper.QuarkAssetObject.AssetPath, new LoadSceneParameters(loadSceneMode));
#else
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(wapper.QuarkAssetObject.AssetName, loadSceneMode);
#endif
            operation.allowSceneActivation = false;
            var hasProviderProgress = progressProvider != null;
            while (!operation.isDone)
            {
                if (hasProviderProgress)
                {
                    var providerProgress = progressProvider();
                    var sum = providerProgress + operation.progress;
                    progress?.Invoke(sum / 2);
                    if (sum >= 1.9)
                    {
                        break;
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
                var hasWapper = GetAssetObjectWapper(scene.Key, ".unity", out var wapper);
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                if (hasWapper)
                {
                    var ao = SceneManager.UnloadSceneAsync(scene.Value);
                    while (!ao.isDone)
                    {
                        overallProgress = overallIndexPercent + (unitResRatio * ao.progress);
                        progress?.Invoke(overallProgress / 100);
                        yield return null;
                    }
                }
                overallProgress = overallIndexPercent + (unitResRatio * 1);
                progress?.Invoke(overallProgress / 100);
            }
            loadedSceneDict.Clear();
            progress?.Invoke(1);
            callback?.Invoke();
        }
        /// <summary>
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var quarkObject = assetData.QuarkAssetObjectList[i];
                if (!quarkAssetObjectDict.TryGetValue(quarkObject.AssetName, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetObjectWapper>();
                    var wapper = new QuarkAssetObjectWapper();
                    wapper.QuarkAssetObject = assetData.QuarkAssetObjectList[i];
                    lnk.AddLast(wapper);
                    quarkAssetObjectDict.Add(quarkObject.AssetName, lnk);
                }
                else
                {
                    var wapper = new QuarkAssetObjectWapper();
                    wapper.QuarkAssetObject = assetData.QuarkAssetObjectList[i];
                    lnkList.AddLast(wapper);
                }
            }
        }
    }
}
