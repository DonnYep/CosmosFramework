using Cosmos.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos
{
    public interface IResourceManager: IModuleManager
    {
        /// <summary>
        ///  资源模块加载资源的模式；
        /// </summary>
        ResourceLoadMode LoadMode { get; }
        /// <summary>
        /// AssetBundle资源加载根路径
        /// </summary>
        string AssetBundleRootPath { get;  }
        /// <summary>
        /// 所有AssetBundle资源包清单的名称
        /// </summary>
        string AssetBundleManifestName { get; }
        /// <summary>
        /// 设置默认设置加载器;
        /// 此方法会使加载模式变为Resource；
        /// </summary>
        void SetDefaultLoader();
        /// <summary>
        /// 设置加载器
        /// </summary>
        /// <param name="loadMode">加载模式</param>
        /// <param name="assetBundleRootPath">资源加载的完全路径</param>
        /// <param name="manifestName">AB包清单名称</param>
        void SetLoader(ResourceLoadMode loadMode, string assetBundleRootPath, string manifestName);
        /// <summary>
        /// 设置AssetBundle资源根路径（仅当使用AssetBundle加载时有效）
        /// </summary>
        /// <param name="path">AssetBundle资源根路径</param>
        void SetAssetBundlePath(string path);
        /// <summary>
        /// 通过名称获取指定的AssetBundle
        /// </summary>
        /// <param name="assetBundleName">名称</param>
        /// <returns>AssetBundle</returns>
        AssetBundle GetAssetBundle(string assetBundleName);
        /// <summary>
        /// 特性无效！；
        /// 加载资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T LoadAsset<T>(AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 特性无效！；
        /// 加载资源（同步）；
        /// 注意：AB环境下会获取bundle中所有T类型的对象；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <returns>资源</returns>
        T[] LoadAllAsset<T>(AssetInfo info) where T : UnityEngine.Object;
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(Type type, bool instantiate = false);
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab<T>(bool instantiate = false) where T : class;
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（同步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        GameObject LoadPrefab(AssetInfo info, bool instantiate = false);
        /// <summary>
        /// 特性无效！；
        /// 加载资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadAssetAsync<T>(AssetInfo info, Action<T> loadDoneCallback, Action<float> loadingCallback = null) where T : UnityEngine.Object;
        /// <summary>
        /// 特性无效！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(AssetInfo info, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <param name="type">类对象类型</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync(Type type, Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false);
        /// <summary>
        /// 特性加载:PrefabAssetAttribute！；
        /// 加载预制体资源（异步）；
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件，T表示原始对象，GameObject表示实例化的对象</param>
        /// <param name="instantiate">是否实例化对象</param>
        /// <returns>加载协程</returns>
        Coroutine LoadPrefabAsync<T>(Action<GameObject> loadDoneCallback, Action<float> loadingCallback = null, bool instantiate = false) where T : class;
        /// <summary>
        /// 加载场景（异步）
        /// </summary>
        /// <param name="info">资源信息标记</param>
        /// <param name="loadingCallback">加载中事件</param>
        /// <param name="loadDoneCallback">加载完成事件</param>
        /// <returns>加载协程迭代器</returns>
        Coroutine LoadSceneAsync(SceneInfo info, Action loadDoneCallback, Action<float> loadingCallback = null);
        /// <summary>
        /// 卸载资源（卸载AssetBundle）
        /// </summary>
        /// <param name="assetBundleName">AB包名称</param>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAsset(string assetBundleName, bool unloadAllLoadedObjects = false);
        /// <summary>
        /// 卸载所有资源（卸载AssetBundle）
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时卸载所有实体对象</param>
        void UnLoadAllAsset(bool unloadAllLoadedObjects = false);
    }
}
