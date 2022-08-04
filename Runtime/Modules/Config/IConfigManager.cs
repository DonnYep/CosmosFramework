namespace Cosmos.Config
{
    //================================================
    /*
     * 1、配置模块，用户存储初始化需存放的全局数据；
    */
    //================================================
    public interface IConfigManager: IModuleManager
    {
        /// <summary>
        /// 添加一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="configValue">存档值</param>
        /// <returns>添加结果</returns>
        bool AddConfig(string configName, string configValue);
        /// <summary>
        /// 添加一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="boolValue">bool值</param>
        /// <param name="intValue">int值</param>
        /// <param name="floatValue">float值</param>
        /// <param name="stringValue">string值</param>
        /// <returns>添加结果</returns>
        bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue);
        /// <summary>
        /// 移除一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>移除结果</returns>
        bool RemoveConfig(string configName);
        /// <summary>
        /// 是否存在配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>存在结果</returns>
        bool HasConfig(string configName);
        /// <summary>
        /// 从指定全局配置项中读取布尔值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的布尔值</returns>
        bool GetBool(string configName);
        /// <summary>
        /// 从指定全局配置项中读取布尔值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的布尔值</returns>
        bool GetBool(string configName, bool defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取整数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的整数值</returns>
        int GetInt(string configName);
        /// <summary>
        /// 从指定全局配置项中读取整数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        int GetInt(string configName, int defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的浮点数值</returns>
        float GetFloat(string configName);
        /// <summary>
        /// 从指定全局配置项中读取浮点数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        float GetFloat(string configName, float defaultValue);
        /// <summary>
        /// 从指定全局配置项中读取字符串值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的字符串值</returns>
        string GetString(string configName);
        /// <summary>
        /// 从指定全局配置项中读取字符串值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        string GetString(string configName, string defaultValue);
        /// <summary>
        /// 移除所有配置数据；
        /// </summary>
        void RemoveAllConfig();
    }
}
