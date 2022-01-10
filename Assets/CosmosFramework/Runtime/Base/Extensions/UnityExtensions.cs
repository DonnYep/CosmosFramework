using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Cosmos
{
    /// <summary>
    /// 组件扩展脚本，封装的一些常用的功能函数
    /// </summary>
    public static class UnityExtensions
    {
        public static T As<T>(this object @this) where T : class
        {
            return @this as T;
        }
        public static T CastTo<T>(this object @this) where T : class
        {
            return (T)@this;
        }
        /// <summary>
        /// 设置并对其到父对象；
        /// </summary>
        public static void SetAlignParent(this Transform @this,Transform parent)
        {
            @this.SetParent(parent);
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
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
        public static RectTransform RectTransform(this GameObject @this)
        {
            return @this.GetComponent<RectTransform>();
        }
        public static T GetOrAddComponent<T>(this Transform @this) where T : Component
        {
            return GetOrAddComponent<T>(@this.gameObject);
        }
        public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
        {
            T component = @this.GetComponent<T>();
            if (component == null)
            {
                component = @this.AddComponent<T>();
            }
            return component;
        }
        public static Component GetOrAddComponent(this GameObject @this, Type type)
        {
            Component component = @this.GetComponent(type);
            if (component == null)
            {
                component = @this.AddComponent(type);
            }
            return component;
        }
        public static Component GetOrAddComponent(this Transform @this, Type type)
        {
            return GetOrAddComponent(@this.gameObject, type);
        }
        public static T GetComponentInParent<T>(this GameObject @this, string parentName)
where T : Component
        {
            return GetComponentInParent<T>(@this.transform, parentName);
        }
        public static T GetComponentInParent<T>(this Transform @this, string parentName)
    where T : Component
        {
            var parent = @this.GetComponentsInParent<Transform>();
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
        public static T GetOrAddComponentInParent<T>(this GameObject @this, string parentName)
    where T : Component
        {
            return GetOrAddComponentInParent<T>(@this.transform, parentName);
        }
        public static T GetOrAddComponentInParent<T>(this Transform @this, string parentName)
    where T : Component
        {
            var parent = @this.GetComponentsInParent<Transform>();
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
            if (comp == null)
            {
                comp = @this.gameObject.AddComponent<T>();
            }
            return comp;
        }
        public static T GetComponentInChildren<T>(this GameObject @this, string childName)
            where T : Component
        {
            return GetComponentInChildren<T>(@this.transform, childName);
        }
        public static T GetComponentInChildren<T>(this Transform @this, string childName)
    where T : Component
        {
            var childs = @this.GetComponentsInChildren<Transform>();
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
        public static T GetComponentInChildren<T>(this Component @this, string childName)
where T : Component
        {
            var childs = @this.GetComponentsInChildren<Transform>();
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
        public static T GetOrAddComponentInChildren<T>(this GameObject @this, string childName)
    where T : Component
        {
            return GetOrAddComponentInChildren<T>(@this.transform, childName);
        }
        public static T GetOrAddComponentInChildren<T>(this Transform @this, string childName)
    where T : Component
        {
            var childs = @this.GetComponentsInChildren<Transform>();
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
            if (comp == null)
                comp = childTrans.gameObject.AddComponent<T>();
            return comp;
        }
        public static T GetComponentInPeer<T>(this GameObject @this, string peerName)
where T : Component
        {
            return GetComponentInPeer<T>(@this.transform, peerName);
        }
        public static T GetComponentInPeer<T>(this Transform @this, string peerName)
    where T : Component
        {
            Transform tran = @this.parent.Find(peerName);
            if (tran != null)
            {
                return tran.GetComponent<T>();
            }
            return null;
        }
        public static T GetOrAddComponentInPeer<T>(this GameObject @this, string peerName)
where T : Component
        {
            return GetOrAddComponentInPeer<T>(@this.transform, peerName);
        }
        public static T GetOrAddComponentInPeer<T>(this Transform @this, string peerName)
    where T : Component
        {
            Transform tran = @this.parent.Find(peerName);
            if (tran != null)
            {
                var comp = tran.GetComponent<T>();
                if (comp == null)
                    @this.gameObject.AddComponent<T>();
                return comp;
            }
            return null;
        }
        public static T[] GetComponentsInPeer<T>(this Transform @this, bool includeSrc = false)
where T : Component
        {
            Transform parentTrans = @this.parent;
            var childTrans = parentTrans.GetComponentsInChildren<Transform>();
            var length = childTrans.Length;
            Transform[] trans;
            if (!includeSrc)
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
        public static T[] GetComponentsInPeer<T>(this GameObject @this, bool includeSrc = false)
where T : Component
        {
            return GetComponentsInPeer<T>(@this.transform, includeSrc);
        }
        public static void DestroyAllChilds(this Transform @this)
        {
            var childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        public static void DestroyAllChilds(this GameObject @this)
        {
            DestroyAllChilds(@this.transform);
        }
        public static Vector2 ConvertToVector2(this Vector3 @this)
        {
            return new Vector2(@this.x, @this.z);
        }
        /// <summary>
        ///返回 (x, 0, y)
        /// </summary>
        public static Vector3 ConvertToVector3(this Vector2 @this)
        {
            return new Vector3(@this.x, 0f, @this.y);
        }
        public static Vector3 ConvertToVector3(this Vector2 @this, float y)
        {
            return new Vector3(@this.x, y, @this.y);
        }
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
        public static Sprite ConvertToSprite(this Texture2D @this)
        {
            Sprite sprite = Sprite.Create(@this, new Rect(0, 0, @this.width, @this.height), Vector2.zero);
            return sprite;
        }
        public static Texture2D Clone(this Texture2D @this)
        {
            Texture2D newTex;
            newTex = new Texture2D(@this.width, @this.height);
            Color[] colors = @this.GetPixels(0, 0, @this.width, @this.height);
            newTex.SetPixels(colors);
            newTex.Apply();
            return newTex;
        }
        public static Texture2D ConvertToSprite(this Sprite @this)
        {
            var newTex = new Texture2D((int)@this.rect.width, (int)@this.rect.height);
            var pixels = @this.texture.GetPixels(
                (int)@this.textureRect.x,
                (int)@this.textureRect.y,
                (int)@this.textureRect.width,
                (int)@this.textureRect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }
        public static void Reset(this AudioSource @this)
        {
            @this.clip = null;
            @this.mute = false;
            @this.playOnAwake = true;
            @this.loop = false;
            @this.priority = 128;
            @this.volume = 1;
            @this.pitch = 1;
            @this.panStereo = 0;
            @this.spatialBlend = 0;
            @this.reverbZoneMix = 1;
            @this.dopplerLevel = 1;
            @this.spread = 0;
            @this.maxDistance = 500;
        }


        /// <summary>
        /// 优化的设置SetActive方法，可以节约重复设置Active的开销
        /// </summary>
        public static void SetActiveOptimize(this GameObject @this, bool isActive)
        {
            if (@this.activeSelf != isActive)
            {
                @this.SetActive(isActive);
            }
        }
        /// <summary>
        /// 获取动画组件切换进度
        /// </summary>
        public static float GetCrossFadeProgress(this Animator @this, int layer = 0)
        {
            if (@this.GetNextAnimatorStateInfo(layer).shortNameHash == 0)
            {
                return 1;
            }

            return @this.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
        }
        public static void EnableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.color = new Color(c.r, c.g, c.b, 1);
            }
        }
        public static void DisableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.sprite = null;
                @this.color = new Color(c.r, c.g, c.b, 0);
            }
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
        /// 设置层级；
        /// 此API会令对象下的所有子对象都被设置层级； 
        /// </summary>
        public static void SetLayer(this GameObject  @this, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            var trans = @this.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans)
            {
                t.gameObject.layer = layer;
            }
        }
        #region UGUI
        public static Button AddButtonClickListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.transform.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonDownListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.transform.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerDown);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonUpListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.transform.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerUp);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.transform.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button RemoveListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.transform.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveListener(handle);
            return @this;
        }
        public static Button RemoveAllListeners(this Button @this, EventTriggerType triggerType)
        {
            var eventTrigger = @this.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveAllListeners();
            return @this;
        }
        #endregion
    }
}