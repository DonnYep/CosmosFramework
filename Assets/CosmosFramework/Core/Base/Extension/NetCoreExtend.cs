using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 此静态类为扩展方法工具类；
/// </summary>
public static class NetCoreExtend
{
    /// <summary>
    /// 字典扩展方法，来自移植.NetCore 2.2
    /// </summary>
    public static bool TryAdd<TKey,TValue>(this Dictionary<TKey,TValue>dict,TKey key,TValue value )
    {
        if (dict.ContainsKey(key))
            return false;
        else
        {
             dict.Add(key, value);
            return true;
        }
    }
    /// <summary>
    /// 字典扩展方法，来自移植.NetCore 2.2
    /// </summary>
    public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict,TKey key, out TValue value)
    {
        if (dict.ContainsKey(key))
        {
            value = dict[key];
            dict.Remove(key);
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
}
