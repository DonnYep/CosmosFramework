using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源模块（资源桥）。
    /// <para>职责分为三层：</para>
    /// <para>1、<b>Provider 管理</b>：注册、选择、切换具体资源方案（Addressables / YooAsset / QuarkAsset / 编辑器兜底）；</para>
    /// <para>2、<b>生命周期</b>：句柄式 API + name-keyed 兼容层，统一引用计数与释放；</para>
    /// <para>3、<b>诊断</b>：与具体方案无关的加载记录、耗时与泄漏检测。</para>
    /// <para>业务模块（UI / Entity / Audio / DataTable / ObjectsPool）只依赖本接口，不依赖任何三方资源库。</para>
    /// </summary>
    public interface IResourceManager : IModuleManager
    {
        #region Provider
        /// <summary>
        /// 当前 Provider
        /// </summary>
        IResourceProvider Provider { get; }
        /// <summary>
        /// 当前 Provider 名称
        /// </summary>
        string ProviderName { get; }
        /// <summary>
        /// 资源系统是否就绪（Provider 已创建且初始化完成）
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// 未指定 Provider 时使用的默认名称（默认 "AssetDatabase"）
        /// </summary>
        string DefaultProviderName { get; set; }
        /// <summary>
        /// 注册一个 Provider 工厂。编辑器兜底 Provider 会在程序集加载时自行注册。
        /// </summary>
        /// <param name="name">注册名（用于配置与日志）</param>
        /// <param name="factory">工厂方法</param>
        void RegisterProvider(string name, Func<IResourceProvider> factory);
        /// <summary>
        /// 是否已注册指定 Provider。
        /// </summary>
        bool HasProvider(string name);
        /// <summary>
        /// 按注册名或类型名（程序集限定名）选择 Provider 并初始化。
        /// <para>切换 Provider 会先释当前 Provider 的全部资源。</para>
        /// </summary>
        /// <param name="name">注册名或类型名</param>
        /// <param name="settings">"key=value;key2=value2" 形式的参数</param>
        void SetProvider(string name, string settings = null);
        /// <summary>
        /// 直接设置 Provider 实例并初始化。
        /// </summary>
        void SetProvider(IResourceProvider provider, string settings = null);
        /// <summary>
        /// 释放并移除当前 Provider。
        /// </summary>
        void UnsetProvider();
        #endregion

        #region 句柄 API（推荐）
        /// <summary>
        /// 异步加载资产。
        /// </summary>
        AssetHandle<T> LoadAssetHandle<T>(string assetKey, uint priority = 0) where T : Object;
        /// <summary>
        /// 异步加载资产及其子资产。
        /// </summary>
        SubAssetsHandle<T> LoadSubAssetsHandle<T>(string assetKey, uint priority = 0) where T : Object;
        /// <summary>
        /// 异步加载根标识下的全部资产。
        /// </summary>
        AllAssetsHandle<T> LoadAllAssetsHandle<T>(string assetKey, uint priority = 0) where T : Object;
        /// <summary>
        /// 批量异步加载（同一标识只发起一次底层请求）。
        /// </summary>
        AssetsHandle<T> LoadAssetsHandle<T>(IEnumerable<string> assetKeys, uint priority = 0) where T : Object;
        /// <summary>
        /// 异步加载场景。
        /// </summary>
        SceneHandle LoadSceneHandle(string sceneKey, bool additive = true, bool activateOnLoad = true, uint priority = 0);
        /// <summary>
        /// 异步实例化（需 Provider 支持 <see cref="ResourceCapabilities.Instance"/>，释放句柄即销毁实例）。
        /// </summary>
        GameObjectHandle InstantiateHandle(string assetKey, Transform parent = null, uint priority = 0);
        #endregion

        #region 模块兼容 API（Coroutine + name-keyed）
        /// <summary>
        /// 异步加载资产，完成后回调；每调用一次增加一个引用计数，需配对调用 <see cref="UnloadAsset"/>。
        /// </summary>
        Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, Action<float> progress = null) where T : Object;
        /// <summary>
        /// 异步加载资产（运行时类型），完成后回调。
        /// </summary>
        Coroutine LoadAssetAsync(string assetName, Type type, Action<Object> callback, Action<float> progress = null);
        /// <summary>
        /// 异步加载资产及其子资产，完成后回调。
        /// </summary>
        Coroutine LoadMainAndSubAssetsAsync<T>(string assetName, Action<T[]> callback, Action<float> progress = null) where T : Object;
        /// <summary>
        /// 异步加载资产及其子资产（运行时类型），完成后回调。
        /// </summary>
        Coroutine LoadMainAndSubAssetsAsync(string assetName, Type type, Action<Object[]> callback, Action<float> progress = null);
        /// <summary>
        /// 异步加载预制体，完成后回调；<paramref name="instantiate"/> 为 true 时额外实例化一份。
        /// </summary>
        Coroutine LoadPrefabAsync(string assetName, Action<GameObject> callback, Action<float> progress = null, bool instantiate = false);
        /// <summary>
        /// 异步加载根标识下的全部资产，完成后回调。
        /// </summary>
        Coroutine LoadAllAssetAsync(string assetPack, Action<Object[]> callback, Action<float> progress = null);
        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetInfo">场景信息</param>
        /// <param name="progressProvider">自定义进度来源（为空则使用加载进度）</param>
        /// <param name="progress">进度回调</param>
        /// <param name="condition">加载完成后的额外等待条件</param>
        /// <param name="callback">场景就绪回调</param>
        Coroutine LoadSceneAsync(SceneAssetInfo sceneAssetInfo, Func<float> progressProvider, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        Coroutine UnloadSceneAsync(SceneAssetInfo sceneAssetInfo, Action<float> progress, Func<bool> condition, Action callback);
        /// <summary>
        /// 卸载一个资源（减少一个引用计数）。
        /// </summary>
        void UnloadAsset(string assetName);
        /// <summary>
        /// 卸载多个资源。
        /// </summary>
        void UnloadAssets(IEnumerable<string> assetNames);
        /// <summary>
        /// 释放全部资源与场景。
        /// </summary>
        /// <param name="unloadAllLoadedObjects">是否同时触发 Resources.UnloadUnusedAssets</param>
        void UnloadAllAsset(bool unloadAllLoadedObjects);
        #endregion

        #region 诊断
        /// <summary>
        /// 是否开启加载诊断（编辑器默认开启）。
        /// </summary>
        bool DiagnosticsEnabled { get; set; }
        /// <summary>
        /// 是否捕获调用栈（定位泄漏源头，有少量开销）。
        /// </summary>
        bool CaptureStackTrace { get; set; }
        /// <summary>
        /// 历史加载记录。
        /// </summary>
        IReadOnlyList<ResourceLoadRecord> LoadRecords { get; }
        /// <summary>
        /// 清空加载记录。
        /// </summary>
        void ClearDiagnostics();
        /// <summary>
        /// 当前运行时状态（调试窗口用）。
        /// </summary>
        ResourceRuntimeInfo GetRuntimeInfo();
        /// <summary>
        /// 生成诊断摘要文本。
        /// </summary>
        string GenerateDiagnosticsReport();
        #endregion
    }
}
