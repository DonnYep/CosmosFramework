using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    /// <summary>
    /// 组件扩展脚本，封装的一些常用的功能函数
    /// </summary>
    public static class Extensions
    { 
        public static void RestWorldTransform(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.rotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
        }
        public static void RestLocalTransform(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
        }
    }
}