using System.Collections.Generic;
using System;
namespace Cosmos.Config
{
    //================================================
    /*
     * 1、配置模块，用户存储初始化需存放的全局数据；
    */
    //================================================
    [Module]
    internal sealed partial class ConfigManager : Module, IConfigManager
    {
        Dictionary<string, ConfigData> configDataDict;
        /// <summary>
        /// 添加一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="configValue">存档值</param>
        /// <returns>添加结果</returns>
        public bool AddConfig(string configName, string configValue)
        {
            bool boolValue = false;
            bool.TryParse(configValue, out boolValue);

            int intValue = 0;
            int.TryParse(configValue, out intValue);

            float floatValue = 0f;
            float.TryParse(configValue, out floatValue);

            return AddConfig(configName, boolValue, intValue, floatValue, configValue);
        }
        /// <summary>
        /// 添加一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="boolValue">bool值</param>
        /// <param name="intValue">int值</param>
        /// <param name="floatValue">float值</param>
        /// <param name="stringValue">string值</param>
        /// <returns>添加结果</returns>
        public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            if (HasConfig(configName))
                return false;
            configDataDict.Add(configName, new ConfigData(boolValue, intValue, floatValue, stringValue));
            return true;
        }
        /// <summary>
        /// 移除一个配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>移除结果</returns>
        public bool RemoveConfig(string configName)
        {
            if (HasConfig(configName))
            {
                configDataDict.Remove(configName);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 是否存在配置数据；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>存在结果</returns>
        public bool HasConfig(string configName)
        {
            return configDataDict.ContainsKey(configName);
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }

            return configData.Value.BoolValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string configName, bool defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.BoolValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }
            return configData.Value.IntValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string configName, int defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.IntValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }

            return configData.Value.FloatValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string configName, float defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.FloatValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }
            return configData.Value.StringValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值；
        /// </summary>
        /// <param name="configName">要获取全局配置项的Key</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string configName, string defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.StringValue : defaultValue;
        }
        /// <summary>
        /// 移除所有配置数据；
        /// </summary>
        public void RemoveAllConfig()
        {
            configDataDict.Clear();
        }
        protected override void OnInitialization()
        {
            configDataDict = new Dictionary<string, ConfigData>();
        }
        ConfigData? GetConfigData(string configName)
        {
            ConfigData data;
            if (configDataDict.TryGetValue(configName, out data))
                return data;
            return null;
        }
    }
}