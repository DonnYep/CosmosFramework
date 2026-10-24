namespace Cosmos.Resource
{
    /// <summary>
    /// 资产加载方式。
    /// <para>一次加载请求的形态，Provider 按此决定调用哪条底层 API。</para>
    /// </summary>
    public enum AssetLoadMode
    {
        /// <summary>
        /// 单个资产
        /// </summary>
        SingleAsset,
        /// <summary>
        /// 资产及其子资产
        /// </summary>
        SubAssets,
        /// <summary>
        /// 全部资产
        /// </summary>
        AllAssets,
        /// <summary>
        /// 场景
        /// </summary>
        Scene,
        /// <summary>
        /// 实例化（底层支持实例化 API 的 Provider 可复用其对象池，如 Addressables.InstantiateAsync）
        /// </summary>
        Instance,
    }
}
