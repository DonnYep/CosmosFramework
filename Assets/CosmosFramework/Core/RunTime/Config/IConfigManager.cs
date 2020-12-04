using Cosmos.Config;
namespace Cosmos
{
    public  interface IConfigManager: IModuleManager
    {
        bool AddConfig(string cfgKey, string configValue);
        bool AddConfig(string cfgKey, bool boolValue, int intValue, float floatValue, string stringValue);
        bool RemoveConfig(string cfgKey);
        bool HasConfig(string cfgKey);
        ConfigData? GetConfigData(string cfgKey);
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetConfigBool(string cfgKey);
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetConfigBool(string cfgKey, bool defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的整数值。</returns>
        int GetConfigInt(string cfgKey);
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        int GetConfigInt(string cfgKey, int defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetConfigFloat(string cfgKey);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetConfigFloat(string cfgKey, float defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的字符串值。</returns>
        string GetConfigString(string cfgKey);
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        string GetConfigString(string cfgKey, string defaultValue);
        void RemoveAllConfig();
    }
}
