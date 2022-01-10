namespace Cosmos.Config
{
    public  interface IConfigManager: IModuleManager
    {
        bool AddConfig(string configName, string configValue);
        bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue);
        bool RemoveConfig(string configName);
        bool HasConfig(string configName);
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetBool(string configName);
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetBool(string configName, bool defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <returns>读取的整数值。</returns>
        int GetInt(string configName);
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        int GetInt(string configName, int defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetFloat(string configName);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetFloat(string configName, float defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <returns>读取的字符串值。</returns>
        string GetString(string configName);
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        string GetString(string configName, string defaultValue);
        void RemoveAllConfig();
    }
}
