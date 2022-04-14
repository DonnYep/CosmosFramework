using Quark.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Quark.Loader
{
    internal class QuarkAssetDatabaseLoader : IQuarkAssetLoader
    {
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObjectWapper]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetObjectWapper>> assetDatabaseMap
            = new Dictionary<string, LinkedList<QuarkAssetObjectWapper>>();
        /// <summary>
        /// Key : [ABName] ; Value : [QuarkAssetBundle]
        /// </summary>
        Dictionary<string, QuarkAssetBundle> assetBundleDict = new Dictionary<string, QuarkAssetBundle>();

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
            SetAssetDatabaseModeData(loaderData as QuarkAssetDataset);
        }
        public T LoadAsset<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(wapper.QuarkAssetObject.AssetPath);
                IncrementQuarkObjectInfo(wapper);
                return asset;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public GameObject LoadPrefab(string assetName, string assetExtension, bool instantiate = false)
        {
            var resGGo = LoadAsset<GameObject>(assetName, assetExtension);
            if (instantiate)
                return GameObject.Instantiate(resGGo);
            else
                return resGGo;
        }
        public T[] LoadAssetWithSubAssets<T>(string assetName, string assetExtension) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetDatabaseObject<T>(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(wapper.QuarkAssetObject.AssetPath);
                var length = assetObj.Length;
                T[] assets = new T[length];
                for (int i = 0; i < length; i++)
                {
                    assets[i] = assetObj[i] as T;
                }
                return assets;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public Coroutine LoadPrefabAsync(string assetName, string assetExtension, Action<GameObject> callback, bool instantiate = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadPrefabAsync(assetName, assetExtension, callback, instantiate));
        }
        public Coroutine LoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive = false)
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadSceneAsync(sceneName, progress, callback, additive));
        }
        public Coroutine LoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAssetWithSubAssetsAsync(assetName, assetExtension, callback));
        }
        public Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, assetExtension, callback));
        }
        public void UnloadAsset(string assetName, string assetExtension)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            var hasWapper = GetAssetDatabaseObject(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                DecrementQuarkObjectInfo(wapper);
            }
        }
        public void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            QuarkUtility.LogInfo("AssetDatabase Mode UnLoadAllAsset");
        }
        public void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            QuarkUtility.LogInfo("AssetDatabase Mode UnLoadAllAsset");
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
            {
                return wapper.GetQuarkAssetObjectInfo();
            }
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo GetInfo(string assetName, string assetExtension)
        {
            var hasWapper = GetAssetDatabaseObject(assetName, assetExtension, out var wapper);
            if (hasWapper)
            {
                return wapper.GetQuarkAssetObjectInfo();
            }
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo[] GetAllLoadedInfos()
        {
            return hashQuarkAssetObjectInfoDict.Values.ToArray();
        }
        IEnumerator EnumLoadAssetWithSubAssetsAsync<T>(string assetName, string assetExtension, Action<T[]> callback)
    where T : UnityEngine.Object
        {
            var assets = LoadAssetWithSubAssets<T>(assetName, assetExtension);
            yield return null;
            callback?.Invoke(assets);
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, string assetExtension, Action<T> callback) where T : UnityEngine.Object
        {
            var asset = LoadAsset<T>(assetName, assetExtension);
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
        IEnumerator EnumLoadSceneAsync(string sceneName, Action<float> progress, Action callback, bool additive)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                QuarkUtility.LogError("Scene name is invalid!");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (assetDatabaseMap == null)
            {
                QuarkUtility.LogError("QuarkAsset 未执行 build 操作！");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (loadedSceneDict.ContainsKey(sceneName))
            {
                QuarkUtility.LogError($"Scene：{sceneName} is already loaded !");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            var hasWapper = GetAssetDatabaseObject(sceneName, ".unity", out var wapper);
            if (hasWapper)
            {
                LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
#if UNITY_EDITOR
                var ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(wapper.QuarkAssetObject.AssetPath, new LoadSceneParameters(loadSceneMode));
#else
                var ao = SceneManager.LoadSceneAsync(wapper.QuarkAssetObject.AssetName, loadSceneMode);
#endif
                while (!ao.isDone)
                {
                    progress?.Invoke(ao.progress);
                    yield return null;
                }
                var scene = SceneManager.GetSceneByPath(wapper.QuarkAssetObject.AssetPath);
                loadedSceneDict.Add(sceneName, scene);
                IncrementQuarkObjectInfo(wapper);
            }
            progress?.Invoke(1);
            callback?.Invoke();
        }
        IEnumerator EnumUnLoadSceneAsync(string sceneName, Action<float> progress, Action callback)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                QuarkUtility.LogError("Scene name is invalid!");
                progress?.Invoke(1);
                callback?.Invoke();
                yield break;
            }
            if (assetDatabaseMap == null)
            {
                QuarkUtility.LogError("QuarkAsset 未执行 build 操作！");
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
            var hasWapper = GetAssetDatabaseObject(sceneName, ".unity", out var wapper);
            if (hasWapper)
            {
                var ao = SceneManager.UnloadSceneAsync(scene);
                while (!ao.isDone)
                {
                    progress?.Invoke(ao.progress);
                    yield return null;
                }
                loadedSceneDict.Remove(sceneName);
                DecrementQuarkObjectInfo(wapper);
            }
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
                var hasWapper = GetAssetDatabaseObject(scene.Key, ".unity", out var wapper);
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
            callback?.Invoke();
        }
        bool GetAssetDatabaseObject<T>(string assetName, string assetExtension, out QuarkAssetObjectWapper wapper)
        {
            wapper = null;
            var typeString = typeof(T).ToString();
            if (assetDatabaseMap.TryGetValue(assetName, out var abLnk))
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
            if (assetDatabaseMap.TryGetValue(assetName, out var abLnk))
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
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var quarkObject = assetData.QuarkAssetObjectList[i];
                if (!assetDatabaseMap.TryGetValue(quarkObject.AssetName, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetObjectWapper>();
                    var wapper = new QuarkAssetObjectWapper();
                    wapper.QuarkAssetObject = assetData.QuarkAssetObjectList[i];
                    lnk.AddLast(wapper);
                    assetDatabaseMap.Add(quarkObject.AssetName, lnk);
                }
                else
                {
                    var wapper = new QuarkAssetObjectWapper();
                    wapper.QuarkAssetObject = assetData.QuarkAssetObjectList[i];
                    lnkList.AddLast(wapper);
                }
            }
        }
        /// <summary>
        /// 增加一个引用计数；
        /// </summary>
        void IncrementQuarkObjectInfo(QuarkAssetObjectWapper wapper)
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
        /// <summary>
        /// 减少一个引用计数；
        /// </summary>
        void DecrementQuarkObjectInfo(QuarkAssetObjectWapper wapper)
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
    }
}
