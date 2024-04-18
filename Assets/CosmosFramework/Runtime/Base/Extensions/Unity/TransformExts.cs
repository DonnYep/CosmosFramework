using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public static class TransformExts
    {
        public static void SetPositionX(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.x = newValue;
            @this.position = v;
        }
        public static void SetPositionY(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.y = newValue;
            @this.position = v;
        }
        public static void SetPositionZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.z = newValue;
            @this.position = v;
        }
        public static void AddPositionX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.x += deltaValue;
            @this.position = v;
        }
        public static void AddPositionY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.y += deltaValue;
            @this.position = v;
        }
        public static void AddPositionZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.z += deltaValue;
            @this.position = v;
        }
        public static void SetLocalPositionX(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.x = newValue;
            @this.localPosition = v;
        }
        public static void SetLocalPositionY(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.y = newValue;
            @this.localPosition = v;
        }
        public static void SetLocalPositionZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.z = newValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.x += deltaValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.y += deltaValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.z += deltaValue;
            @this.localPosition = v;
        }
        public static void SetLocalScaleX(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.x = newValue;
            @this.localScale = v;
        }
        public static void SetLocalScaleY(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.y = newValue;
            @this.localScale = v;
        }
        public static void SetLocalScaleZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.z = newValue;
            @this.localScale = v;
        }
        public static void AddLocalScaleX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.x += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 y 分量。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">y 分量增量。</param>
        public static void AddLocalScaleY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.y += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 z 分量。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">z 分量增量。</param>
        public static void AddLocalScaleZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.z += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 二维空间下使 <see cref="Transform" /> 指向指向目标点的算法，使用世界坐标。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点。</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" />。</remarks>
        public static void LookAt2D(this Transform @this, Vector2 lookAtPoint2D)
        {
            Vector3 vector = lookAtPoint2D.ConvertToVector3() - @this.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                @this.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }
        public static void DeleteChildrens(this Transform @this)
        {
            var childCount = @this.childCount;
            if (childCount == 0)
                return;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        public static Transform FindChildren(this Transform @this, string name)
        {
            int childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = @this.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
                Transform result = FindChildren(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        /// <summary>
        /// 查询所有包含指定名称的子物体以及孙物体
        /// </summary>
        /// <param name="this">父物体</param>
        /// <param name="name">包含的名称</param>
        /// <param name="findActiveObjectOnly">是否只查找active状态的对象</param>
        /// <returns>查询到的子物体</returns>
        public static List<Transform> FindChildrenContainsName(this Transform @this, string name, bool findActiveObjectOnly = false)
        {
            List<Transform> childList = new List<Transform>();
            int childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = @this.GetChild(i);
                if (findActiveObjectOnly)
                {
                    if (!child.gameObject.activeSelf)
                        continue;
                    if (child.name.Contains(name))
                    {
                        childList.Add(child);
                    }
                }
                else
                {
                    if (child.name.Contains(name))
                    {
                        childList.Add(child);
                    }
                }
                childList.AddRange(FindChildrenContainsName(child, name));
            }
            return childList;
        }
        /// <summary>
        /// 查找所有子物体孙物体等
        /// </summary>
        /// <param name="this">父物体</param>
        /// <param name="findActiveObjectOnly">是否只查找active状态的对象</param>
        /// <returns>查询到的子物体</returns>
        public static List<Transform> FindAllChildren(this Transform @this, bool findActiveObjectOnly = false)
        {
            List<Transform> childList = new List<Transform>();
            int childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = @this.GetChild(i);
                if (findActiveObjectOnly)
                {
                    if (child.gameObject.activeSelf)
                    {
                        childList.Add(child);
                    }
                }
                else
                {
                    childList.Add(child);
                }
                childList.AddRange(FindAllChildren(child, findActiveObjectOnly));
            }
            return childList;
        }
        /// <summary>
        /// 查找同级别其他对象
        /// </summary>
        /// <param name="this">同级别当前对象</param>
        /// <param name="includeSelf">是否包含本身</param>
        /// <returns>同级的对象</returns>
        public static Transform[] FindPeers(this Transform @this, bool includeSelf = false)
        {
            Transform parentTrans = @this.parent;
            var childTrans = parentTrans.GetComponentsInChildren<Transform>();
            var length = childTrans.Length;
            if (!includeSelf)
                return Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != @this);
            else
                return Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
        }
        public static Transform FindPeer(this Transform @this, string name)
        {
            Transform tran = @this.parent.Find(name);
            if (tran == null)
                return null;
            return tran;
        }
        public static Transform FindParent(this Transform @this, string name)
        {
            Transform parent = @this.parent;
            while (parent != null)
            {
                if (parent.name == name)
                {
                    break;
                }
                parent = parent.parent;
            }
            return parent;
        }
        public static Transform[] FindParents(this Transform @this, Predicate<Transform> condition)
        {
            List<Transform> transformList = new List<Transform>();
            Transform parent = @this.parent;
            while (parent != null)
            {
                if (condition(parent))
                {
                    transformList.Add(parent);
                }
                parent = parent.parent;
            }
            return transformList.ToArray();
        }
        /// <summary>
        /// 查找同级别下所有目标组件，略耗性能
        /// </summary>
        /// <typeparam name="T">目标组件</typeparam>
        /// <param name="this">同级别当前对象</param>
        /// <param name="includeSelf">是否包含当前对象</param>
        /// <returns>同级别对象数组</returns>
        public static T[] PeerComponets<T>(this Transform @this, bool includeSelf = false) where T : Component
        {
            Transform parentTrans = @this.parent;
            var childTrans = parentTrans.GetComponentsInChildren<Transform>();
            var length = childTrans.Length;
            Transform[] trans;
            if (!includeSelf)
                trans = Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
            else
                trans = Utility.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != @this);
            var transLength = trans.Length;
            T[] src = new T[transLength];
            int idx = 0;
            for (int i = 0; i < transLength; i++)
            {
                var comp = trans[i].GetComponent<T>();
                if (comp != null)
                {
                    src[idx] = comp;
                    idx++;
                }
            }
            T[] dst = new T[idx];
            Array.Copy(src, 0, dst, 0, idx);
            return dst;
        }
        public static void DestroyAllChilds(this Transform @this)
        {
            var childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        public static void ResetWorldTransform(this Transform @this)
        {
            @this.position = Vector3.zero;
            @this.rotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }
        public static void ResetLocalTransform(this Transform @this)
        {
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }
        public static void ResetRectTransform(this RectTransform @this)
        {
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
            @this.offsetMax = Vector2.zero;
            @this.offsetMin = Vector2.zero;
        }
        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(this Transform @this)
        {
            if (@this == null)
                return;
            for (int i = @this.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 获取第一个子物体
        /// </summary>
        /// <param name="findActiveObjectOnly">是否只查找active状态的对象</param>
        public static Transform GetFirstChild(this Transform @this, bool findActiveObjectOnly = false)
        {
            if (@this == null || @this.childCount == 0)
                return null;
            for (int i = 0; i < @this.childCount; i++)
            {
                Transform target = @this.GetChild(i);
                if (findActiveObjectOnly)
                {
                    if (target.gameObject.activeSelf)
                        return target;
                }
                else
                    return target;
            }
            return null;
        }
        /// <summary>
        /// 获取最后一个子物体
        /// </summary>
        /// <param name="findActiveObjectOnly">是否只查找active状态的对象</param>
        public static Transform GetLastChild(this Transform @this, bool findActiveObjectOnly = false)
        {
            if (@this == null || @this.childCount == 0)
                return null;
            for (int i = @this.childCount - 1; i >= 0; i--)
            {
                Transform target = @this.GetChild(i);
                if (findActiveObjectOnly)
                {
                    if (target.gameObject.activeSelf)
                        return target;
                }
                else
                    return target;
            }
            return null;
        }
        /// <summary>
        /// 设置并对齐到父对象
        /// </summary>
        public static void SetAndAlignParent(this Transform @this, Transform parent)
        {
            @this.SetParent(parent);
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }

    }
}
