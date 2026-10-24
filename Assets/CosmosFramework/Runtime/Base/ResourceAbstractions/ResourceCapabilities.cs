using System;

namespace Cosmos.Resource
{
    /// <summary>
    /// Provider 能力位标记。
    /// <para>各三方资源方案的 API 并非完全等价，桥在调用前依据该标记决定是否可用、是否需要降级或直接报错；</para>
    /// <para>缺少某项能力时的行为由桥统一决定，Provider 不需要撒谎去“凑”一个它没有的能力。</para>
    /// </summary>
    [Flags]
    public enum ResourceCapabilities
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 单资产异步加载（所有 Provider 都必须支持）
        /// </summary>
        Asset = 1 << 0,
        /// <summary>
        /// 子资产加载
        /// </summary>
        SubAssets = 1 << 1,
        /// <summary>
        /// 全部资产加载
        /// </summary>
        AllAssets = 1 << 2,
        /// <summary>
        /// 标签/分类加载
        /// </summary>
        Label = 1 << 3,
        /// <summary>
        /// 场景加载
        /// </summary>
        Scene = 1 << 4,
        /// <summary>
        /// 实例化加载（释放句柄即销毁实例）
        /// </summary>
        Instance = 1 << 5,
        /// <summary>
        /// 同步加载
        /// </summary>
        Sync = 1 << 6,
        /// <summary>
        /// 下载/更新
        /// </summary>
        Download = 1 << 7,
        /// <summary>
        /// 加载优先级
        /// </summary>
        Priority = 1 << 8,
    }
}
