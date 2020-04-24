using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using Object = UnityEngine.Object;
namespace Cosmos
{
    public enum MessageColor
    {
        black,
        blue,
        brown,
        darkblue,
        green,//原谅色
        lime,//力量色
        grey,
        fuchsia,//洋红色
        navy,//海军蓝
        orange,
        red,
        teal,//蓝绿色
        yellow,
        maroon,//褐红色
        purple
    }
    /// <summary>
    /// 通用工具类：
    /// 数组工具，反射工具，文字工具，加密工具，
    /// 数学工具，持久化数据工具，Debug工具，Editor工具等等
    /// </summary>
    public static partial class Utility
    {
        public static void DebugLog(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color=#254FDB>" + o + "</color></b>");
            else
                Debug.Log("<b>-->><color=#254FDB>" + o + "</color></b>", context);
#endif
        }
        public static void DebugLog(object o, MessageColor messageColor, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.Log("<b>-->><color=" + messageColor.ToString() + ">" + o + "</color></b>");
            else
                Debug.Log("<b>-->><color=" + messageColor.ToString() + ">" + o + "</color></b>", context);
#endif
        }
        public static void DebugWarning(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>");
            else
                Debug.LogWarning("<b>-->><color=#FF5E00>" + o + "</color></b>", context);
#endif
        }
        public static void DebugError(object o, Object context = null)
        {
#if UNITY_EDITOR
            if (!ApplicationConst.Editor.EnableDebugLog)
                return;
            if (context == null)
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>");
            else
                Debug.LogError("<b>-->><color=#FF0000>" + o + "</color></b>", context);
#endif
        }
        /// <summary>
        ///谨慎使用这个方法
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect(); Resources.UnloadUnusedAssets();
        }
    }
}