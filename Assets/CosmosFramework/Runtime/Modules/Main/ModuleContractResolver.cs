using System;
using System.Collections.Generic;
using System.Linq;
namespace Cosmos
{
    /// <summary>
    /// 模块契约（IXxxManager）解析；
    /// <para>模块的类型与其对外契约原本靠"接口名包含类名"的字符串约定，容易误判。
    /// 这里改为只在 <see cref="IModuleManager"/> 派生接口里找，按下列顺序匹配：</para>
    /// <para>1、精确名 I{类名}（如 UIManager → IUIManager）；</para>
    /// <para>2、接口名以类名结尾（如 AudioManager → ICoreAudioManager）；</para>
    /// <para>3、只有一个候选接口时直接采用；</para>
    /// <para>4、仍然歧义 → 直接报错（不再"随便挑一个"）。</para>
    /// <para>解析结果按类型缓存，进程内只算一次。</para>
    /// </summary>
    internal static class ModuleContractResolver
    {
        static readonly Dictionary<Type, Type> contractCache = new Dictionary<Type, Type>();
        /// <summary>
        /// 解析模块契约；没有任何 <see cref="IModuleManager"/> 派生接口时返回 null。
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>契约接口类型</returns>
        public static Type Resolve(Type moduleType)
        {
            if (moduleType == null)
                throw new ArgumentNullException("Module type is invalid !");
            if (contractCache.TryGetValue(moduleType, out var cachedContract))
                return cachedContract;
            var contract = ResolveUncached(moduleType);
            contractCache[moduleType] = contract;
            return contract;
        }
        /// <summary>
        /// 清空解析缓存；模块表清空时调用。
        /// </summary>
        public static void Clear()
        {
            contractCache.Clear();
        }
        static Type ResolveUncached(Type moduleType)
        {
            var candidates = moduleType.GetInterfaces()
                .Where(contract => contract != typeof(IModuleManager) && typeof(IModuleManager).IsAssignableFrom(contract))
                .ToArray();
            if (candidates.Length == 0)
                return null;

            var moduleTypeName = moduleType.Name;
            var exactName = "I" + moduleTypeName;
            var exactMatches = candidates.Where(contract => contract.Name == exactName).ToArray();
            if (exactMatches.Length == 1)
                return exactMatches[0];

            var suffixMatches = candidates.Where(contract => contract.Name.EndsWith(moduleTypeName, StringComparison.Ordinal)).ToArray();
            if (suffixMatches.Length == 1)
                return suffixMatches[0];

            if (candidates.Length == 1)
                return candidates[0];

            throw new ArgumentException(
                $"Module : {moduleType} implements multiple module contracts ({string.Join(", ", candidates.Select(contract => contract.Name))}) " +
                $"and none of them can be chosen unambiguously. Rename the intended contract to '{exactName}', " +
                $"or keep only one interface derived from {typeof(IModuleManager).Name}.");
        }
    }
}
