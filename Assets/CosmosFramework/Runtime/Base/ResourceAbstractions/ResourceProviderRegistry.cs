using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// Provider 全局注册表。
    /// <para>适配器程序集可在 <c>[RuntimeInitializeOnLoadMethod]</c> 中自行注册，</para>
    /// <para>因此 CosmosConfig 只需要写一个名字（如 "Addressables" / "YooAsset" / "QuarkAsset"），</para>
    /// <para>不需要在运行时手动 new 出 Provider，也不需要框架引用三方程序集。</para>
    /// <code>
    /// // 适配器内部
    /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    /// static void Register() =&gt; ResourceProviderRegistry.Register("Addressables", () =&gt; new AddressablesProvider());
    /// </code>
    /// </summary>
    public static class ResourceProviderRegistry
    {
        static readonly Dictionary<string, Func<IResourceProvider>> factories = new Dictionary<string, Func<IResourceProvider>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 注册（同名覆盖）。
        /// </summary>
        public static void Register(string name, Func<IResourceProvider> factory)
        {
            if (string.IsNullOrEmpty(name) || factory == null)
                return;
            factories[name] = factory;
        }
        /// <summary>
        /// 注销。
        /// </summary>
        public static void Unregister(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            factories.Remove(name);
        }
        /// <summary>
        /// 是否已注册。
        /// </summary>
        public static bool Contains(string name)
        {
            return !string.IsNullOrEmpty(name) && factories.ContainsKey(name);
        }
        /// <summary>
        /// 创建实例。
        /// </summary>
        public static bool TryCreate(string name, out IResourceProvider provider)
        {
            provider = null;
            if (string.IsNullOrEmpty(name) || !factories.TryGetValue(name, out var factory) || factory == null)
                return false;
            provider = factory();
            return provider != null;
        }
        /// <summary>
        /// 已注册的全部名称。
        /// </summary>
        public static IEnumerable<string> Names
        {
            get { return factories.Keys; }
        }
        /// <summary>
        /// 清空（退出播放模式/销毁时）。
        /// </summary>
        public static void Clear()
        {
            factories.Clear();
        }
    }
}
