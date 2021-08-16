using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    /// <summary>
    /// 组件扩展脚本，封装的一些常用的功能函数
    /// </summary>
    public static class UnityExtensions
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
        public static RectTransform RectTransform(this GameObject go)
        {
            return go.GetComponent<RectTransform>();
        }
        public static T CastTo<T>(this object source) where T : class
        {
            return source as T;
        }
        /// <summary>
        /// 获取或增加组件。
        /// </summary>
        /// <typeparam name="T">要获取或增加的组件。</typeparam>
        /// <param name="gameObject">目标对象。</param>
        /// <returns>获取或增加的组件。</returns>
        public static T GetOrAddComponent<T>(this Transform  transform) where T : Component
        {
            return GetOrAddComponent<T>(transform.gameObject);
        }
        /// <summary>
        /// 获取或增加组件。
        /// </summary>
        /// <param name="gameObject">目标对象。</param>
        /// <param name="type">要获取或增加的组件类型。</param>
        /// <returns>获取或增加的组件。</returns>
        public static Component GetOrAddComponent(this Transform  transform, Type type)
        {
            return GetOrAddComponent(transform.gameObject, type);
        }
        /// <summary>
        /// 获取或增加组件。
        /// </summary>
        /// <typeparam name="T">要获取或增加的组件。</typeparam>
        /// <param name="gameObject">目标对象。</param>
        /// <returns>获取或增加的组件。</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
        /// <summary>
        /// 获取或增加组件。
        /// </summary>
        /// <param name="gameObject">目标对象。</param>
        /// <param name="type">要获取或增加的组件类型。</param>
        /// <returns>获取或增加的组件。</returns>
        public static Component GetOrAddComponent(this GameObject gameObject, Type type)
        {
            Component component = gameObject.GetComponent(type);
            if (component == null)
            {
                component = gameObject.AddComponent(type);
            }
            return component;
        }
        public static T GetComponentInParent<T>(this GameObject gameObject, string parentName)
            where T : Component
        {
            return GetComponentInParent<T>(gameObject.transform, parentName);
        }
        public static T GetComponentInParent<T>(this Transform transform, string parentName)
    where T : Component
        {
            var parent = transform.GetComponentsInParent<Transform>();
            var length = parent.Length;
            Transform parentTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (parent[i].name == parentName)
                {
                    parentTrans = parent[i];
                    break;
                }
            }
            if (parentTrans == null)
                return null;
            var comp = parentTrans.GetComponent<T>();
            return comp;
        }
        public static T GetComponentInChildren<T>(this GameObject gameObject, string childName)
            where T : Component
        {
            return GetComponentInChildren<T>(gameObject.transform, childName);
        }
        public static T GetComponentInChildren<T>(this Transform transform, string childName)
    where T : Component
        {
            var childs = transform.GetComponentsInChildren<Transform>();
            var length = childs.Length;
            Transform childTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (childs[i].name == childName)
                {
                    childTrans = childs[i];
                    break;
                }
            }
            if (childTrans == null)
                return null;
            var comp = childTrans.GetComponent<T>();
            return comp;
        }
        public static Vector2 ConvertToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
        /// <summary>
        ///返回 (x, 0, y)
        /// </summary>
        public static Vector3 ConvertToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0f, vector2.y);
        }
        public static Vector3 ConvertToVector3(this Vector2 vector2, float y)
        {
            return new Vector3(vector2.x, y, vector2.y);
        }
        public static void SetPositionX(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.x = newValue;
            transform.position = v;
        }
        public static void SetPositionY(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.y = newValue;
            transform.position = v;
        }
        public static void SetPositionZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.position;
            v.z = newValue;
            transform.position = v;
        }
        public static void AddPositionX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.x += deltaValue;
            transform.position = v;
        }
        public static void AddPositionY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.y += deltaValue;
            transform.position = v;
        }
        public static void AddPositionZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.position;
            v.z += deltaValue;
            transform.position = v;
        }
        public static void SetLocalPositionX(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.x = newValue;
            transform.localPosition = v;
        }
        public static void SetLocalPositionY(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.y = newValue;
            transform.localPosition = v;
        }
        public static void SetLocalPositionZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.localPosition;
            v.z = newValue;
            transform.localPosition = v;
        }
        public static void AddLocalPositionX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.x += deltaValue;
            transform.localPosition = v;
        }
        public static void AddLocalPositionY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.y += deltaValue;
            transform.localPosition = v;
        }
        public static void AddLocalPositionZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localPosition;
            v.z += deltaValue;
            transform.localPosition = v;
        }
        public static void SetLocalScaleX(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.x = newValue;
            transform.localScale = v;
        }
        public static void SetLocalScaleY(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.y = newValue;
            transform.localScale = v;
        }
        public static void SetLocalScaleZ(this Transform transform, float newValue)
        {
            Vector3 v = transform.localScale;
            v.z = newValue;
            transform.localScale = v;
        }
        public static void AddLocalScaleX(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.x += deltaValue;
            transform.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 y 分量。
        /// </summary>
        /// <param name="transform"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">y 分量增量。</param>
        public static void AddLocalScaleY(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.y += deltaValue;
            transform.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 z 分量。
        /// </summary>
        /// <param name="transform"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">z 分量增量。</param>
        public static void AddLocalScaleZ(this Transform transform, float deltaValue)
        {
            Vector3 v = transform.localScale;
            v.z += deltaValue;
            transform.localScale = v;
        }
        /// <summary>
        /// 二维空间下使 <see cref="Transform" /> 指向指向目标点的算法，使用世界坐标。
        /// </summary>
        /// <param name="transform"><see cref="Transform" /> 对象。</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点。</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" />。</remarks>
        public static void LookAt2D(this Transform transform, Vector2 lookAtPoint2D)
        {
            Vector3 vector = lookAtPoint2D.ConvertToVector3() - transform.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                transform.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }
        public static Sprite ConvertToSprite(this Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return sprite;
        }
        public static Texture2D Clone(this Texture2D texture)
        {
            Texture2D newTex;
            newTex = new Texture2D(texture.width, texture.height);
            Color[] colors = texture.GetPixels(0, 0, texture.width, texture.height);
            newTex.SetPixels(colors);
            newTex.Apply();
            return newTex;
        }
        public static Texture2D ConvertToSprite(this Sprite sprite)
        {
            var newTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }
        public static void Reset(this AudioSource audioSource)
        {
            audioSource.clip = null;
            audioSource.mute = false;
            audioSource.playOnAwake = true;
            audioSource.loop = false;
            audioSource.priority = 128;
            audioSource.volume = 1;
            audioSource.pitch = 1;
            audioSource.panStereo = 0;
            audioSource.spatialBlend = 0;
            audioSource.reverbZoneMix = 1;
            audioSource.dopplerLevel = 1;
            audioSource.spread = 0;
            audioSource.maxDistance = 500;
        }
    }
}