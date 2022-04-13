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
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetObject>> assetDatabaseMap
            = new Dictionary<string, LinkedList<QuarkAssetObject>>();
        /// <summary>
        /// Key : [ABName] ; Value : [QuarkAssetBundle]
        /// </summary>
        Dictionary<string, QuarkAssetBundle> assetBundleDict = new Dictionary<string, QuarkAssetBundle>();

        /// <summary>
        /// Hash===QuarkObjectInfo
        /// </summary>
        Dictionary<int, QuarkAssetObjectInfo> hashQuarkObjectInfoDict = new Dictionary<int, QuarkAssetObjectInfo>();
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
            QuarkAssetObject quarkObject = null;
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkObject = GetAssetDatabaseObject(lnk, assetExtension);
            if (quarkObject != null)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(quarkObject.AssetPath);
                IncrementQuarkObjectInfo(quarkObject);
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
            QuarkAssetObject quarkObject = null;
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkObject = GetAssetDatabaseObject(lnk, assetExtension);
            if (quarkObject != null)
            {
                var assetObj = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(quarkObject.AssetPath);
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
            QuarkAssetObject quarkObject = null;
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkObject = GetAssetDatabaseObject(lnk, assetExtension);
            if (quarkObject != null)
            {
                DecrementQuarkObjectInfo(quarkObject);
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
            QuarkAssetObject abObject = null;
            if (assetDatabaseMap.TryGetValue(assetName, out var abLnk))
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
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo GetInfo(string assetName, string assetExtension)
        {
            QuarkAssetObject abObject = null;
            if (assetDatabaseMap.TryGetValue(assetName, out var abLnk))
            {
                if (string.IsNullOrEmpty(assetExtension))
                {
                    abObject = abLnk.First.Value;
                }
                else
                {
                    foreach (var obj in abLnk)
                    {
                        if (obj.AssetExtension == assetExtension)
                        {
                            abObject = obj;
                            break;
                        }
                    }
                }
                if (abObject != null)
                    return hashQuarkObjectInfoDict[abObject.GetHashCode()];
            }
            return QuarkAssetObjectInfo.None;
        }
        public QuarkAssetObjectInfo[] GetAllLoadedInfos()
        {
            return hashQuarkObjectInfoDict.Values.ToArray();
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
            QuarkAssetObject quarkObject = null;
            if (assetDatabaseMap.TryGetValue(sceneName, out var lnk))
                quarkObject = GetAssetDatabaseObject(lnk, ".unity");
            if (quarkObject != null)
            {
                LoadSceneMode loadSceneMode = additive == true ? LoadSceneMode.Additive : LoadSceneMode.Single;
#if UNITY_EDITOR
                var ao = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(quarkObject.AssetPath, new LoadSceneParameters(loadSceneMode));
#else
                var ao = SceneManager.LoadSceneAsync(quarkObject.AssetName, loadSceneMode);
#endif
                while (!ao.isDone)
                {
                    progress?.Invoke(ao.progress);
                    yield return null;
                }
                var scene = SceneManager.GetSceneByPath(quarkObject.AssetPath);
                loadedSceneDict.Add(sceneName, scene);
                IncrementQuarkObjectInfo(quarkObject);
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
            QuarkAssetObject quarkObject = null;
            if (assetDatabaseMap.TryGetValue(sceneName, out var lnk))
                quarkObject = GetAssetDatabaseObject(lnk, ".unity");
            if (quarkObject != null)
            {
                var ao = SceneManager.UnloadSceneAsync(scene);
                while (!ao.isDone)
                {
                    progress?.Invoke(ao.progress);
                    yield return null;
                }
                loadedSceneDict.Remove(sceneName);
                DecrementQuarkObjectInfo(quarkObject);
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
                QuarkAssetObject quarkObject = null;
                if (assetDatabaseMap.TryGetValue(scene.Key, out var lnk))
                    quarkObject = GetAssetDatabaseObject(lnk, ".unity");
                var overallIndexPercent = 100 * ((float)currentSceneIndex / sceneCount);
                currentSceneIndex++;
                if (quarkObject != null)
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
        QuarkAssetObject GetAssetDatabaseObject(LinkedList<QuarkAssetObject> lnk, string assetExtension = null)
        {
            QuarkAssetObject quarkAssetObject = null;
            if (!string.IsNullOrEmpty(assetExtension))
            {
                foreach (var quarkObject in lnk)
                {
                    if (quarkObject.AssetExtension == assetExtension)
                    {
                        quarkAssetObject = quarkObject;
                        break;
                    }
                }
            }
            else
            {
                quarkAssetObject = lnk.First.Value;
            }
            return quarkAssetObject;
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
                    var lnk = new LinkedList<QuarkAssetObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    assetDatabaseMap.Add(quarkObject.AssetName, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
                var info = QuarkAssetObjectInfo.Create(quarkObject.AssetName, quarkObject.AssetPath, null, quarkObject.AssetExtension, 0);
                info.ABObjectHash = quarkObject.GetHashCode();
                hashQuarkObjectInfoDict.Add(info.ABObjectHash, info);
            }
        }
        /// <summary>
        /// 增加一个引用计数；
        /// </summary>
        void IncrementQuarkObjectInfo(QuarkAssetObject quarkObject)
        {
            var hashCode = quarkObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info++;
            //增加一个AB的引用计数；
            if (!assetBundleDict.TryGetValue(quarkObject.AssetBundleName, out var assetBundle))
            {
                assetBundle = new QuarkAssetBundle(quarkObject.AssetBundleName, null);
                assetBundleDict.Add(assetBundle.AssetBundleName, assetBundle);
            }
            assetBundle.ReferenceCount++;
        }
        /// <summary>
        /// 减少一个引用计数；
        /// </summary>
        void DecrementQuarkObjectInfo(QuarkAssetObject quarkObject)
        {
            var hashCode = quarkObject.GetHashCode();
            var info = hashQuarkObjectInfoDict[hashCode];
            hashQuarkObjectInfoDict[hashCode] = info--;
            //减少一个AB的引用计数
            if (assetBundleDict.TryGetValue(quarkObject.AssetBundleName, out var assetBundle))
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
