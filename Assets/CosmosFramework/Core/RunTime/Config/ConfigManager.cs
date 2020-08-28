using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using Cosmos;
using System;

namespace Cosmos.Config
{
    /// <summary>
    /// 载入时候读取配置，例如声音大小，角色等
    /// </summary>
    internal sealed class ConfigManager : Module<ConfigManager>
    {
        Dictionary<ushort, ConfigData> configDataDict = new Dictionary<ushort, ConfigData>();
        internal bool AddConfigData(ushort cfgKey, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            if (HasConfig(cfgKey))
                return false;
            configDataDict.Add(cfgKey, new ConfigData(boolValue, intValue, floatValue, stringValue));
            return true;
        }
        internal bool RemoveConfig(ushort cfgKey)
        {
            if (HasConfig(cfgKey))
            {
                configDataDict.Remove(cfgKey);
                return true;
            }
            else
                return false;
        }
        internal bool HasConfig(ushort cfgKey)
        {
            return configDataDict.ContainsKey(cfgKey);
        }
        internal ConfigData? GetConfigData(ushort cfgKey)
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
        internal bool GetBool(ushort cfgKey)
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
        internal bool GetBool(ushort cfgKey, bool defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.BoolValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取整数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的整数值。</returns>
        internal int GetInt(ushort cfgKey)
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
        internal int GetInt(ushort cfgKey, int defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.IntValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取浮点数值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的浮点数值。</returns>
        internal float GetFloat(ushort cfgKey)
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
        internal float GetFloat(ushort cfgKey, float defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.FloatValue : defaultValue;
        }
        /// <summary>
        /// 从指定全局配置项中读取字符串值。
        /// </summary>
        /// <param name="cfgKey">要获取全局配置项的Key。</param>
        /// <returns>读取的字符串值。</returns>
        internal string GetString(ushort cfgKey)
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
       internal string GetString(ushort cfgKey, string defaultValue)
        {
            ConfigData? configData = GetConfigData(cfgKey);
            return configData.HasValue ? configData.Value.StringValue : defaultValue;
        }
        internal void RemoveAllConfig()
        {
            configDataDict.Clear();
        }
    }
}