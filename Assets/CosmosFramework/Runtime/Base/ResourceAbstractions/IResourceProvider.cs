namespace Cosmos.Resource
{
    /// <summary>
    /// 资源提供者（SPI）。一套三方资源方案（Addressables / YooAsset / QuarkAsset）对应一个实现。
    /// <para>Provider 只负责“怎么加载”和“怎么释放”，不负责：</para>
    /// <para>· 引用计数与句柄生命周期 —— 由 <see cref="ResourceProviderOperation"/> 统一管理；</para>
    /// <para>· 名称索引与模块兼容层 —— 由资源桥（IResourceManager）统一管理；</para>
    /// <para>· 寻址规则 —— key 原样透传，框架不额外发明一层地址映射。</para>
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Provider 名称（用于日志、诊断与配置匹配）。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 支持的能力集合。
        /// </summary>
        ResourceCapabilities Capabilities { get; }
        /// <summary>
        /// 是否初始化完成。
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// 初始化参数。
        /// </summary>
        ResourceInitParameters InitParameters { get; }

        /// <summary>
        /// 异步初始化。
        /// <para>实现方负责启动返回的操作（异步场景调用 OperationSystem.StartOperation）；</para>
        /// <para>同步完成的场景直接调用 Complete 即可，桥会依据 Status 判断是否需要启动。</para>
        /// </summary>
        InitializationOperation InitializeAsync(ResourceInitParameters parameters);

        /// <summary>
        /// 创建一次加载操作。
        /// <para>实现方只需实现“发起请求 / 写回结果 / 释放底层资源”，引用计数由基类处理。</para>
        /// </summary>
        ResourceProviderOperation CreateOperation(ResourceRequest request);

        /// <summary>
        /// 卸载未被引用的资源（底层方案支持时）。
        /// </summary>
        void UnloadUnusedAssets();

        /// <summary>
        /// 销毁 Provider，释放其持有的全部资源与句柄。
        /// </summary>
        void Destroy();
    }
}
