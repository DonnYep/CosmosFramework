using System;
using System.IO;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源信息。由资源桥在请求时构建，是 Provider 与句柄之间传递的最轻量描述。
    /// <para>刻意不包含清单、包体、版本等实现细节——那些概念属于具体 Provider 内部。</para>
    /// </summary>
    [Serializable]
    public sealed class AssetInfo
    {
        /// <summary>
        /// 资源唯一标识，原样透传给 Provider（资源名 / 资源名.后缀 / 资源路径 / Provider 自定义地址）。
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 从 Key 解析出的文件名（不含后缀），用于资源桥的名称索引与日志。
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 从 Key 解析出的后缀（不含点），无后缀时为空。
        /// </summary>
        public string Extension { get; private set; }
        /// <summary>
        /// 该资源由哪个 Provider 提供。
        /// </summary>
        public string ProviderName { get; private set; }
        /// <summary>
        /// 分类标签（Provider 支持标签时可用）。
        /// </summary>
        public string[] Tags { get; private set; }
        /// <summary>
        /// 是否无效（Key 为空时视为无效）。
        /// </summary>
        public bool Invalid
        {
            get { return string.IsNullOrEmpty(Key); }
        }

        AssetInfo() { }

        /// <summary>
        /// 通过资源标识创建资源信息。
        /// </summary>
        /// <param name="key">资源标识</param>
        /// <param name="providerName">所属 Provider 名称</param>
        public AssetInfo(string key, string providerName)
        {
            Key = key;
            ProviderName = providerName;
            ParseKey(key, out var name, out var extension);
            Name = name;
            Extension = extension;
        }
        /// <summary>
        /// 创建带标签的资源信息。
        /// </summary>
        public AssetInfo(string key, string providerName, string[] tags) : this(key, providerName)
        {
            Tags = tags;
        }

        /// <summary>
        /// 解析资源标识中的文件名与后缀。
        /// </summary>
        public static void ParseKey(string key, out string name, out string extension)
        {
            name = key;
            extension = string.Empty;
            if (string.IsNullOrEmpty(key))
                return;
            var normalized = key.Replace('\\', '/');
            var index = normalized.LastIndexOf('/');
            if (index >= 0 && index + 1 < normalized.Length)
                normalized = normalized.Substring(index + 1);
            var dotIndex = normalized.LastIndexOf('.');
            if (dotIndex > 0 && dotIndex + 1 < normalized.Length)
            {
                extension = normalized.Substring(dotIndex + 1);
                name = normalized.Substring(0, dotIndex);
                return;
            }
            name = normalized;
        }

        public override string ToString()
        {
            return $"{Key}({ProviderName})";
        }
    }
}
