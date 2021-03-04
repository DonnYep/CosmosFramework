using System.Collections;
using System.Collections.Generic;
using System;

namespace Cosmos.Config
{
    /// <summary>
    /// 载入时候读取配置，例如声音大小，角色等
    /// </summary>
    [Module]
    internal sealed class ConfigManager : Module, IConfigManager
    {
        Dictionary<string, ConfigData> configDataDict;
        public override void OnInitialization()
        {
            configDataDict = new Dictionary<string, ConfigData>();
        }
        public override void OnPreparatory()
        {
            var assemblies = GameManager.Assemblies;
            var length = assemblies.Length;
            for (int i = 0; i < length; i++)
            {
                var objs = Utility.Assembly.GetInstancesByAttribute<ImplementProviderAttribute, IConfigProvider>(assemblies[i]);
                for (int j = 0; j < objs.Length; j++)
                {
                    try
                    {
                        objs[j].LoadConfig();
                    }
                    catch (Exception e)
                    {
                        Utility.Debug.LogError(e);
                    }
                }
            }
        }
        /// <summary>
        /// 增加指定全局配置项。
        /// </summary>
        /// <param name="cfgKey">要增加全局配置项的名称。</param>
        /// <param name="configValue">全局配置项的值。</param>
        /// <returns>是否增加全局配置项成功。</returns>
        public bool AddConfig(string cfgKey, string configValue)
        {
            bool boolValue = false;
            bool.TryParse(configValue, out boolValue);

            int intValue = 0;
            int.TryParse(configValue, out intValue);

            float floatValue = 0f;
            float.TryParse(configValue, out floatValue);

            return AddConfig(cfgKey, boolValue, intValue, floatValue, configValue);
        }
        public bool AddConfig(string cfgKey, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            if (HasConfig(cfgKey))
                return false;
            configDataDict.Add(cfgKey, new ConfigData(boolValue, intValue, floatValue, stringValue));
            return true;
        }
        public bool RemoveConfig(string cfgKey)
        {
            if (HasConfig(cfgKey))
            {
                configDataDict.Remove(cfgKey);
                return true;
            }
            else
                return false;
        }
        public bool HasConfig(string cfgKey)
        {
            return configDataDict.ContainsKey(cfgKey);
        }
        public ConfigData? GetConfigData(string cfgKey)
        {
            ConfigData data;
            if (configDataDict.TryGetValue(cfgKey, out data))
                return data;
            return null;
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetConfigBool(string cfgKey)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException(Utility.Text.Format("Config name '{0}' is not exist.", cfgKey));
            }

            return configData.Value.BoolValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取布尔值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetConfigBool(string cfgKey, bool defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.BoolValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的整数值。</returns>
        public int GetConfigInt(string cfgKey)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException(Utility.Text.Format("Config name '{0}' is not exist.", cfgKey));
            }

            return configData.Value.IntValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetConfigInt(string cfgKey, int defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.IntValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetConfigFloat(string cfgKey)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException(Utility.Text.Format("Config name '{0}' is not exist.", cfgKey));
            }

            return configData.Value.FloatValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetConfigFloat(string cfgKey, float defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.FloatValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetConfigString(string cfgKey)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException(Utility.Text.Format("Config name '{0}' is not exist.", cfgKey));
            }
            return configData.Value.StringValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <param name="defaultValue">当指定的全局配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetConfigString(string cfgKey, string defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.StringValue : defaultValue;
        }
        public void RemoveAllConfig()
        {
            configDataDict.Clear();
        }
    }
}