using System;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    /// <summary>
    /// Provider 初始化参数。
    /// <para>不同 Provider 需要的参数差异很大（YooAsset 要包名与运行模式，Addressables 几乎不需要，Quark 要 dataset 路径），</para>
    /// <para>因此统一收敛为字符串键值对，由各 Provider 自行解析，桥不做任何解释。</para>
    /// <code>
    /// // CosmosConfig 面板：Addressables；空 settings
    /// // CosmosConfig 面板：YooAsset；package=DefaultPackage;mode=Offline
    /// </code>
    /// </summary>
    [Serializable]
    public class ResourceInitParameters
    {
        /// <summary>
        /// Provider 名称
        /// </summary>
        public string ProviderName;
        /// <summary>
        /// 包裹/分包名（Provider 支持分包时有意义，如 YooAsset 的 Package）
        /// </summary>
        public string PackageName;
        /// <summary>
        /// Provider 自定义参数
        /// </summary>
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        public ResourceInitParameters() { }

        public ResourceInitParameters(string providerName)
        {
            ProviderName = providerName;
        }

        /// <summary>
        /// 取字符串参数。
        /// </summary>
        public string GetString(string key, string defaultValue = null)
        {
            if (Settings == null || string.IsNullOrEmpty(key))
                return defaultValue;
            if (Settings.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
                return value;
            return defaultValue;
        }
        /// <summary>
        /// 取布尔参数（true/1/yes 视为 true，不区分大小写）。
        /// </summary>
        public bool GetBool(string key, bool defaultValue = false)
        {
            var value = GetString(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            value = value.Trim().ToLowerInvariant();
            return value == "true" || value == "1" || value == "yes" || value == "on";
        }
        /// <summary>
        /// 取整数参数。
        /// </summary>
        public int GetInt(string key, int defaultValue = 0)
        {
            var value = GetString(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return int.TryParse(value, out var result) ? result : defaultValue;
        }
        /// <summary>
        /// 取无符号整数参数。
        /// </summary>
        public uint GetUInt(string key, uint defaultValue = 0)
        {
            var value = GetString(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return uint.TryParse(value, out var result) ? result : defaultValue;
        }
        /// <summary>
        /// 取64位整数参数。
        /// </summary>
        public long GetLong(string key, long defaultValue = 0)
        {
            var value = GetString(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return long.TryParse(value, out var result) ? result : defaultValue;
        }
        /// <summary>
        /// 从 "key=value;key2=value2" 形式的文本解析参数。
        /// </summary>
        /// <param name="context">参数文本</param>
        /// <param name="parameters">目标参数对象，为空时新建</param>
        public static ResourceInitParameters Parse(string context, ResourceInitParameters parameters = null)
        {
            parameters = parameters ?? new ResourceInitParameters();
            if (parameters.Settings == null)
                parameters.Settings = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(context))
                return parameters;
            var segments = context.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i].Trim();
                if (segment.Length == 0 || segment.StartsWith("//"))
                    continue;
                var index = segment.IndexOf('=');
                if (index <= 0)
                    continue;
                var key = segment.Substring(0, index).Trim();
                var value = segment.Substring(index + 1).Trim();
                if (key.Length == 0)
                    continue;
                parameters.Settings[key] = value;
            }
            return parameters;
        }
    }
}
