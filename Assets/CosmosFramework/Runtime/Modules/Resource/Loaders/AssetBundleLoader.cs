using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Cosmos.Resource;

namespace Cosmos
{
    public class AssetBundleLoader : IResourceLoadHelper
    {
        public AssetBundleLoader(string assetBundleRootPath, string manifestName)
        {
            assetBundleHashDict = new Dictionary<string, Hash128>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
            this.AssetBundleRootPath = assetBundleRootPath;
            AssetBundleManifestName = manifestName;
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }
        public void SetLoaderData(string assetBundleRootPath, string manifestName)
        {
            this.AssetBundleRootPath = assetBundleRootPath;
            this.AssetBundleManifestName = manifestName;
        }
        public AssetBundleLoader()
        {
            assetBundleHashDict = new Dictionary<string, Hash128>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }
        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        public string AssetBundleRootPath { get; private set; }
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        public string AssetBundleManifestName { get; private set; }

        public bool IsLoading { get { return _isLoading; } }

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
        private bool _isLoading = false;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil _loadWait;
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            AssetBundleRootPath = path;
        }
        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        public AssetBundle GetAssetBundle(string assetBundleName)
        {
            assetBundleDict.TryGetValue(assetBundleName, out var ab);
            return ab;
        }
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
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object
        {
            return Utility.Unity.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback, loadingCallback));
        }
        public Coroutine LoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback = null)
        {
            return Utility.Unity.StartCoroutine(EnumLoadSceneAsync(info, loadDoneCallback, loadingCallback));
        }
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            foreach (var assetBundle in assetBundleDict)
            {
                assetBundle.Value.Unload(unloadAllLoadedObjects);
            }
            assetBundleDict.Clear();
            assetBundleHashDict.Clear();
            AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
        }
        public void UnLoadAsset(object customData, bool unloadAllLoadedObjects = false)
        {
            var assetBundleName = Convert.ToString(customData);
            if (assetBundleDict.ContainsKey(assetBundleName))
            {
                assetBundleDict[assetBundleName].Unload(unloadAllLoadedObjects);
                assetBundleDict.Remove(assetBundleName);
            }
            if (assetBundleHashDict.ContainsKey(assetBundleName))
            {
                assetBundleHashDict.Remove(assetBundleName);
            }
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
                        UnLoadAsset(AssetBundleManifestName);
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
                    if (!request.isNetworkError && !request.isHttpError)
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
                    if (!request.isNetworkError && !request.isHttpError)
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
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator EnumLoadAssetAsync<T>(AssetInfoBase info, Action<T> loadDoneCallback, Action<float> loadingCallback, bool instantiate = false)
            where T : UnityEngine.Object
        {
            DateTime beginTime = DateTime.Now;
            if (_isLoading)
            {
                yield return _loadWait;
            }
            _isLoading = true;
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);
            DateTime waitTime = DateTime.Now;
            UnityEngine.Object asset = null;
            yield return LoadAssetBundleAsync(info.AssetBundleName, loadingCallback);
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
                loadDoneCallback?.Invoke(asset as T);
            }
            _isLoading = false;
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator EnumLoadSceneAsync(SceneAssetInfo info, Action loadDoneCallback, Action<float> loadingCallback)
        {
            if (_isLoading)
            {
                yield return _loadWait;
            }
            _isLoading = true;
            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);
            yield return LoadAssetBundleAsync(info.AssetBundleName, loadingCallback);
            yield return SceneManager.LoadSceneAsync(info.AssetPath, LoadSceneMode.Additive);
            loadDoneCallback?.Invoke();
            _isLoading = false;
        }
    }
}
