using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace Cosmos.Resource
{
    public enum ResourceLoadMode : byte
    {
        Resource = 0,
        AssetBundle = 1,
    }
    [Module]
    internal sealed class ResourceManager : Module , IResourceManager
    {
        #region Properties
        /// <summary>
        /// 当前的资源加载模式
        /// </summary>
        public ResourceLoadMode LoadMode { get; private set; }
        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        string assetBundleRootPath;
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        string assetBundleManifestName;
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
        IMonoManager monoManager;
        #endregion
        #region Methods
        public override void OnInitialization()
        {
            assetBundleHashDict = new Dictionary<string, Hash128>();
            assetBundleDict = new Dictionary<string, AssetBundle>();
        }
        public override void OnPreparatory()
        {
            monoManager = GameManager.GetModule<IMonoManager>();
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public override void OnTermination()
        {
            UnLoadAllAsset(true);
            ClearMemory();
        }
        /// <summary>
        /// 设置加载器
        /// </summary>
        /// <param name="loadMode">加载模式</param>
        /// <param name="assetBundleRootPath">资源加载的完全路径</param>
        /// <param name="manifestName">AB包清单名称</param>
        public void SetLoader(ResourceLoadMode loadMode, string assetBundleRootPath, string manifestName)
        {
            LoadMode = loadMode;
            this.assetBundleRootPath = assetBundleRootPath;
            assetBundleManifestName = manifestName;
            _loadWait = new WaitUntil(() => { return !_isLoading; });
        }
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        public void SetAssetBundlePath(string path)
        {
            assetBundleRootPath = path;
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
        /// <summary>
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback,Action<float> loadingCallback=null )
            where T : UnityEngine.Object
        {
            return monoManager.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback,loadingCallback));
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(Type type,Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
        {
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return monoManager.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback, loadingCallback, instantiate));
            }
            else
                return null;
        }
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync<T>( Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false)
            where T:class
        {
            var type = typeof(T);
            var attribute = type.GetCustomAttribute<PrefabAssetAttribute>();
            if (attribute != null)
            {
                var info = new AssetInfo(attribute.AssetBundleName, attribute.AssetPath, attribute.ResourcePath);
                return monoManager.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback, loadingCallback, instantiate));
            }
            else
                return null;
        }
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        public Coroutine LoadPrefabAsync(AssetInfo info, Action<GameObject> loadDoneCallback,Action<float> loadingCallback=null,  bool instantiate = false)
        {
            return monoManager.StartCoroutine(EnumLoadAssetAsync(info, loadDoneCallback, loadingCallback, instantiate));
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        public Coroutine LoadSceneAsync(SceneInfo info, Action loadDoneCallback, Action<float> loadingCallback=null)
        {
            return monoManager.StartCoroutine(EnumLoadSceneAsync(info, loadDoneCallback, loadingCallback));
        }
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            if (LoadMode == ResourceLoadMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
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
        }
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        public void UnLoadAllAsset(bool unloadAllLoadedObjects = false)
        {
            if (LoadMode == ResourceLoadMode.Resource)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                foreach (var assetBundle in assetBundleDict)
                {
                    assetBundle.Value.Unload(unloadAllLoadedObjects);
                }
                assetBundleDict.Clear();
                assetBundleHashDict.Clear();
                AssetBundle.UnloadAllAssetBundles(unloadAllLoadedObjects);
            }
        }
        /// <summary>
        /// 清理内存，释放空闲内存
        /// </summary>
        void ClearMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        /// <summary>
        /// 异步加载依赖AB包
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadDependenciesAssetBundleAsync(string assetBundleName)
        {
            if (LoadMode == ResourceLoadMode.AssetBundle)
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
            }
            yield return null;
        }
        /// <summary>
        /// 异步加载AB包清单
        /// </summary>
        /// <returns>协程迭代器</returns>
        IEnumerator LoadAssetBundleManifestAsync()
        {
            if (string.IsNullOrEmpty(assetBundleManifestName))
            {
                throw new ArgumentException($"ResourceManager-->>请设置资源管理模块的 Manifest Name 属性，为所有AB包提供依赖清单！");
            }
            else
            {
                if (assetBundleManifest == null)
                {
                    yield return LoadAssetBundleAsync(assetBundleManifestName, true);

                    if (assetBundleDict.ContainsKey(assetBundleManifestName))
                    {
                        assetBundleManifest = assetBundleDict[assetBundleManifestName].LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        UnLoadAsset(assetBundleManifestName);
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
                    ? UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
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
                    ? UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + assetBundleName)
                    : UnityWebRequestAssetBundle.GetAssetBundle(assetBundleRootPath + assetBundleName, GetAssetBundleHash(assetBundleName)))
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
            if (LoadMode == ResourceLoadMode.Resource)
            {
                ResourceRequest request = Resources.LoadAsync<T>(info.ResourcePath);
                while (!request.isDone)
                {
                    loadingCallback?.Invoke(request.progress);
                    yield return null;
                }
                asset = request.asset;
                if (asset == null)
                {
                    throw new ArgumentNullException($"ResourceManager-->>加载资源失败：Resources文件夹中不存在资源 {info.ResourcePath }！");
                }
                else
                {
                    if (instantiate)
                    {
                        asset = GameObject.Instantiate(asset);
                    }
                }
            }
            else
            {
                yield return LoadAssetBundleAsync(info.AssetBundleName, loadingCallback);
                if (assetBundleDict.ContainsKey(info.AssetBundleName))
                {
                    asset = assetBundleDict[info.AssetBundleName].LoadAsset<T>(info.AssetPath);
                    if (asset == null)
                    {
                        throw new ArgumentNullException($"ResourceManager-->>加载资源失败：AB包 {info.AssetBundleName } 中不存在资源 {info.AssetPath } ！");
                    }
                }
            }
            if (asset != null)
            {
                loadDoneCallback?.Invoke(asset as T);
            }
            asset = null;
            _isLoading = false;
        }
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        IEnumerator EnumLoadSceneAsync(SceneInfo info, Action loadDoneCallback, Action<float> loadingCallback)
        {
            DateTime beginTime = DateTime.Now;

            if (_isLoading)
            {
                yield return _loadWait;
            }
            _isLoading = true;

            yield return LoadDependenciesAssetBundleAsync(info.AssetBundleName);

            DateTime waitTime = DateTime.Now;

            if (LoadMode == ResourceLoadMode.Resource)
            {
                throw new ArgumentException($"ResourceManager-->>加载场景失败：场景加载不允许使用Resource模式！");
            }
            else
            {
                yield return LoadAssetBundleAsync(info.AssetBundleName, loadingCallback);
                yield return SceneManager.LoadSceneAsync(info.AssetPath, LoadSceneMode.Additive);
            }
            DateTime endTime = DateTime.Now;
            Utility.Debug.LogInfo($"异步加载场景完成[{LoadMode}模式]：" +
                $"{info.AssetPath}\r\n等待耗时：{(waitTime - beginTime).TotalSeconds}秒  加载耗时：{(endTime - waitTime).TotalSeconds}秒");
            loadDoneCallback?.Invoke();
            _isLoading = false;
        }
        #endregion
    }
}

