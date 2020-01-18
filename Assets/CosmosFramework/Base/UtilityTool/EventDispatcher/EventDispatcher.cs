using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [DisallowMultipleComponent]
    public class EventDispatcher : MonoBehaviour
    {
        [SerializeField] string eventKey="eventKey";
        public string EventKey { get { return eventKey; } }
        public string DispatcherName { get { return this.gameObject.name; } }
        public GameEventArgs EventArg { get; set; }
        public void DispatchEvent()
        {
            Facade.Instance.DispatchEvent(eventKey, this, EventArg);
        }
        /// <summary>
        /// 注销事件，这里由事件分发者注销
        /// </summary>
        public void DeregisterEvent()
        {
            Facade.Instance.DeregisterEvent(eventKey);
        }
    }
}