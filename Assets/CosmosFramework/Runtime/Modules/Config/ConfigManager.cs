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
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            if (HasConfig(configName))
                return false;
            configDataDict.Add(configName, new ConfigData(boolValue, intValue, floatValue, stringValue));
            return true;
        }
        ///<inheritdoc/>
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
        ///<inheritdoc/>
        public bool HasConfig(string configName)
        {
            return configDataDict.ContainsKey(configName);
        }
        ///<inheritdoc/>
        public bool GetBool(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }

            return configData.Value.BoolValue;
        }
        ///<inheritdoc/>
        public bool GetBool(string configName, bool defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.BoolValue : defaultValue;
        }
        ///<inheritdoc/>
        public int GetInt(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }
            return configData.Value.IntValue;
        }
        ///<inheritdoc/>
        public int GetInt(string configName, int defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.IntValue : defaultValue;
        }
        ///<inheritdoc/>
        public float GetFloat(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }

            return configData.Value.FloatValue;
        }
        ///<inheritdoc/>
        public float GetFloat(string configName, float defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.FloatValue : defaultValue;
        }
        ///<inheritdoc/>
        public string GetString(string configName)
        {
            ConfigData? configData = GetConfigData(configName);
            if (!configData.HasValue)
            {
                throw new ArgumentNullException($"Config name '{configName}' is not exist!");
            }
            return configData.Value.StringValue;
        }
        ///<inheritdoc/>
        public string GetString(string configName, string defaultValue)
        {
            ConfigData? configData = GetConfigData(configName);
            return configData.HasValue ? configData.Value.StringValue : defaultValue;
        }
        ///<inheritdoc/>
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