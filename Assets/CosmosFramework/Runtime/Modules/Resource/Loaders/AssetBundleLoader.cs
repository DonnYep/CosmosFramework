using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
namespace Cosmos.Resource
{
    //TODO AssetBundleLoader 需要重写
    public class AssetBundleLoader : IResourceLoadHelper
    {

        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        public string AssetBundleRootPath { get; private set; }
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        public string AssetBundleManifestName { get; private set; }
        public bool IsProcessing { get { return isProcessing; } }
        /// <summary>
        /// 缓存的所有AssetBundle包【AB包名称、AB包】
        /// </summary>
        Dictionary<string, AssetBundle> assetBundleDict;
        /// <summary>
        /// 所有AssetBundle资源包清单
        /// </summary>
        AssetBundleManifest assetBundleManifest;
        /// <summary>
        /// 所有AssetBundle的Hash128值【AB包名称、Hash128值】
        /// </summary>
        Dictionary<string, Hash128> assetBundleHashDict;
        /// <summary>
        /// 单线下载中
        /// </summary>
        private bool isProcessing = false;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil loadWait;
        public AssetBundleLoader()
        {
            assetBundleHashDict = new Dictionary<string, Hash128>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
            loadWait = new WaitUntil(() => { return !isProcessing; });
        }
        public AssetBundleLoader(string assetBundleRootPath, string manifestName)
        {
            assetBundleHashDict = new Dictionary<string, Hash128>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
            this.AssetBundleRootPath = assetBundleRootPath;
            AssetBundleManifestName = manifestName;
            loadWait = new WaitUntil(() => { return !isProcessing; });
        }
        public void SetLoaderData(string assetBundleRootPath, string manifestName)
        {
            this.AssetBundleRootPath = assetBundleRootPath;
            this.AssetBundleManifestName = manifestName;
        }
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            AssetBundleRootPath = path;
        }
        /// <inheritdoc/>
        public T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            T asset = null;
            if (assetBundleDict.ContainsKey(info.AssetBundleName))
            {
                asset = assetBundleDict[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                if (asset == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                }
            }
            return asset;
        }
        /// <inheritdoc/>
        public T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object
        {
            T[] asset = null;
            if (assetBundleDict.ContainsKey(info.AssetBundleName))
            {
                asset = assetBundleDict[info.AssetBundleName].LoadAllAssets<T>();
                if (asset == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                }
            }
            return asset;
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(info, callback, progress));
        }
        /// <inheritdoc/>
        public void UnloadAsset(AssetInfo info)
        {
        }
        /// <inheritdoc/>
        public void UnloadAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            assetBundleHashDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
        }
        /// <inheritdoc/>
        public T[] LoadAssetWithSubAssets<T>(AssetInfo info) where T : UnityEngine.Object
        {
            T[] assets = null;
            if (assetBundleDict.ContainsKey(info.AssetBundleName))
            {
                assets = assetBundleDict[info.AssetBundleName].LoadAssetWithSubAssets<T>(info.AssetPath);
                if (assets == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                }
            }
            return assets;
        }
        /// <inheritdoc/>
        public Coroutine LoadAssetWithSubAssetsAsync<T>(AssetInfo info, Action<T[]> callback, Action<float> progress = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetWithSubAssets(info, callback, progress));
        }
        /// <inheritdoc/>
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumLoadSceneAsync(info, progressProvider, progress, condition, callback));
        }
        /// <inheritdoc/>
        public Coroutine UnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            return Utility.Unity.StartCoroutine(EnumUnloadSceneAsync(info, progress, condition, callback));
        }
        /// <summary>
        /// 异步加载依赖AB包
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            yield return LoadAssetBundleManifestAsync();

            if (assetBundleManifest != null)
            {
                string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
                foreach (string item in dependencies)
                {
                    if (assetBundleDict.ContainsKey(item))
                    {
                        continue;
                    }

                    yield return LoadAssetBundleAsync(item);
                }
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadAssetBundleManifestAsync()
        {
            if (string.IsNullOrEmpty(AssetBundleManifestName))
            {
                throw new ArgumentException($"ResourceManager-->>请设置资源管理模块的 Manifest Name 属性，为所有AB包提供依赖清单！");
            }
            else
            {
                if (assetBundleManifest == null)
                {
                    yield return LoadAssetBundleAsync(AssetBundleManifestName, true);

                    if (assetBundleDict.ContainsKey(AssetBundleManifestName))
                    {
                        assetBundleManifest = assetBundleDict[AssetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnloadAssetBundle(AssetBundleManifestName);
                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="isManifest">是否是加载清单</param>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadAssetBundleAsync(string assetBundleName, bool isManifest = false)
        {
            if (!assetBundleDict.ContainsKey(assetBundleName))
            {
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    yield return request.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                    if (!request.isNetworkError && !request.isHttpError)
#endif
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            assetBundleDict.Add(assetBundleName, bundle);
                        }
                        else
                        {
                            throw new ArgumentException($"ResourceManager-->>请求：{request.url }未下载到AB包！");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"ResourceManager-->>请求：{request.url } 遇到网络错误：{ request.error }！");
                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包（提供进度回调）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="loadingAction">加载中事件</param>
        /// <param name="isManifest">是否是加载清单</param>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadAssetBundleAsync(string assetBundleName, Action<float> loadingAction, bool isManifest = false)
        {
            if (!assetBundleDict.ContainsKey(assetBundleName))
            {
                using (UnityWebRequest request = isManifest
                    ? UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        loadingAction?.Invoke(request.downloadProgress);
                        yield return null;
                    }
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
#elif UNITY_2018_1_OR_NEWER
                    if (!request.isNetworkError && !request.isHttpError)
#endif
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                        if (bundle)
                        {
                            assetBundleDict.Add(assetBundleName, bundle);
                        }
                        else
                        {
                            throw new ArgumentException($"ResourceManager-->>请求：{request.url }未下载到AB包！");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"ResourceManager-->>请求：{request.url }遇到网络错误：{request.error }！");
                    }
                }
            }
            yield return null;
        }
        /// <summary>
        /// 获取AB包的hash值
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <returns>hash值</returns>
        Hash128 GetAssetBundleHash(string assetBundleName)
        {
            if (assetBundleHashDict.ContainsKey(assetBundleName))
            {
                return assetBundleHashDict[assetBundleName];
            }
            else
            {
                Hash128 hash = assetBundleManifest.GetAssetBundleHash(assetBundleName);
                assetBundleHashDict.Add(assetBundleName, hash);
                return hash;
            }
        }
        /// <summary>
        /// 特性无效！；
        /// 加载并实例化资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="progress">加载中事件</param>
        /// <param name="callback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator EnumLoadAssetWithSubAssets<T>(AssetInfoBase info, Action<T[]> callback, Action<float> progress)
            where T : UnityEngine.Object
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);
            DateTime waitTime = DateTime.Now;
            T[] assets = null;
            yield return LoadAssetBundleAsync(info.AssetBundleName, progress);
            if (assetBundleDict.ContainsKey(info.AssetBundleName))
            {
                assets = assetBundleDict[info.AssetBundleName].LoadAssetWithSubAssets<T>(info.AssetPath);
                if (assets == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                }
            }
            if (assets != null)
            {
                callback?.Invoke(assets);
            }
            isProcessing = false;
        }
        IEnumerator EnumLoadAssetAsync<T>(AssetInfoBase info, Action<T> callback, Action<float> progress, bool instantiate = false)
            where T : UnityEngine.Object
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);
            DateTime waitTime = DateTime.Now;
            UnityEngine.Object asset = null;
            yield return LoadAssetBundleAsync(info.AssetBundleName, progress);
            if (assetBundleDict.ContainsKey(info.AssetBundleName))
            {
                asset = assetBundleDict[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                if (asset == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                }
                else
                {
                    if (instantiate)
                    {
                        asset = GameObject.Instantiate(asset);
                    }
                }
            }
            if (asset != null)
            {
                callback?.Invoke(asset as T);
            }
            isProcessing = false;
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">加载场景进度回调</param>
        /// <param name="condition">场景加载完成的条件</param>
        /// <param name="callback">场景加载完毕回调</param>
        /// <returns>协程对象</returns>
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);
            yield return LoadAssetBundleAsync(info.AssetBundleName, progress);
            {
                isProcessing = true;
                var ao = SceneManager.LoadSceneAsync(info.SceneName, (LoadSceneMode)Convert.ToByte(info.Additive));
                ao.allowSceneActivation = false;
                var hasProviderProgress = progressProvider != null;
                while (!ao.isDone)
                {
                    if (hasProviderProgress)
                    {
                        var providerProgress = progressProvider();
                        var sum = providerProgress + ao.progress;
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
                        progress?.Invoke(ao.progress);
                        if (ao.progress >= 0.9f)
                        {
                            break;
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
                progress?.Invoke(1);
                if (condition != null)
                    yield return new WaitUntil(condition);
                ao.allowSceneActivation = true;
                callback?.Invoke();
                isProcessing = false;
            }
        }
        /// <summary>
        /// 卸载场景（异步）
        /// </summary>
        /// <param name="info">资源信息</param>
        /// <param name="progress">卸载场景的进度</param>
        /// <param name="condition">卸载场景完成的条件</param>
        /// <param name="callback">场景卸载完毕后的回调<</param>
        /// <returns>协程对象</returns>
        IEnumerator EnumUnloadSceneAsync(SceneAssetInfo info, Action<float> progress, Func<bool> condition, Action callback)
        {
            if (isProcessing)
            {
                yield return loadWait;
            }
            isProcessing = true;
            var ao = SceneManager.UnloadSceneAsync(info.SceneName);
            while (!ao.isDone)
            {
                progress?.Invoke(ao.progress);
                if (ao.progress >= 0.9f)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            if (condition != null)
                yield return new WaitUntil(condition);
            progress?.Invoke(1);
            callback?.Invoke();
            isProcessing = false;
        }
        void UnloadAssetBundle(string assetBundleName)
        {
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assetBundleDict[assetBundleName].Unload(false);
                assetBundleDict.Remove(assetBundleName);
            }
            if (assetBundleHashDict.ContainsKey(assetBundleName))
            {
                assetBundleHashDict.Remove(assetBundleName);
            }
        }
    }
}
