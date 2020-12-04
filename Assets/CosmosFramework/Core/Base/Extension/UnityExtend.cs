using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// 组件扩展脚本，封装的一些常用的功能函数
    /// </summary>
    public static class UnityExtend
    {
        public static void ResetWorldTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
        }
        public static void ResetLocalTransform(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
        }
        public static void ResetRectTransform(this RectTransform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
            trans.offsetMax = Vector2.zero;
            trans.offsetMin = Vector2.zero;
        }
        /// <summary>
        /// 转换为标准时间字符串（yyyy/MM/dd HH:mm:ss）
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <returns>字符串</returns>
        public static string ToDefaultDateString(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public static RectTransform RectTransform(this  GameObject go)
        {
            return go.GetComponent<RectTransform>();
        }
        public static T Convert<T>(this object source)where T:class
        {
            return source as T;
        }
    }
}