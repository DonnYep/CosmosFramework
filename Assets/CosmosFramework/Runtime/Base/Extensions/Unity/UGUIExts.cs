using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cosmos
{
    public static class UGUIExts
    {
        /// <summary>
        /// 监听点击按下事件。
        /// </summary>
        /// <param name="this">扩展对象</param>
        /// <param name="handle">监听的方法</param>
        /// <returns>UIBehaviour </returns>
        public static UIBehaviour AddClickDownListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return AddEventListener(@this, EventTriggerType.PointerDown, handle);
        }
        /// <summary>
        /// 监听点击事件。
        /// <para>此事件的执行优先级高于 <see cref="AddClickUpListener"/></para> 
        /// <para>UGUI的执行优先级<see cref="EventTriggerType.PointerClick"/> > <see cref="EventTriggerType.PointerUp"/></para> 
        /// </summary>
        /// <param name="this">扩展对象</param>
        /// <param name="handle">监听的方法</param>
        /// <returns>UIBehaviour </returns>
        public static UIBehaviour AddClickListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return AddEventListener(@this, EventTriggerType.PointerClick, handle);
        }
        /// <summary>
        /// 监听点击抬起事件。
        /// <para>此事件的执行优先级低于 <see cref="AddClickListener"/></para> 
        /// <para>UGUI的执行优先级<see cref="EventTriggerType.PointerClick"/> > <see cref="EventTriggerType.PointerUp"/></para> 
        /// </summary>
        /// <param name="this">扩展对象</param>
        /// <param name="handle">监听的方法</param>
        /// <returns>UIBehaviour </returns>
        public static UIBehaviour AddClickUpListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return AddEventListener(@this, EventTriggerType.PointerUp, handle);
        }
        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="this">扩展对象</param>
        /// <param name="triggerType">事件类型</param>
        /// <param name="handle">监听的方法</param>
        /// <returns>UIBehaviour </returns>
        public static UIBehaviour AddEventListener(this UIBehaviour @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static UIBehaviour RemoveClickDownListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return RemoveEventListener(@this, EventTriggerType.PointerDown, handle);
        }
        public static UIBehaviour RemoveClickListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return RemoveEventListener(@this, EventTriggerType.PointerClick, handle);
        }
        public static UIBehaviour RemoveClickUpListener(this UIBehaviour @this, UnityAction<BaseEventData> handle)
        {
            return RemoveEventListener(@this, EventTriggerType.PointerUp, handle);
        }
        public static UIBehaviour RemoveEventListener(this UIBehaviour @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveListener(handle);
            return @this;
        }
        public static UIBehaviour RemoveAllEventListeners(this UIBehaviour @this, EventTriggerType triggerType)
        {
            var eventTrigger = @this.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveAllListeners();
            return @this;
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
        /// 在指定物体上添加指定图片
        /// </summary>
        public static Image AddImage(this GameObject @this, Sprite sprite)
        {
            @this.SetActive(false);
            Image image = @this.GetComponent<Image>();
            if (!image)
                image = @this.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            @this.SetActive(true);
            return image;
        }
    }
}
