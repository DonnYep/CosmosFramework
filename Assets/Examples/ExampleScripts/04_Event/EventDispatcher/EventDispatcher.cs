using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [DisallowMultipleComponent]
    public class EventDispatcher : MonoBehaviour
    {
        /// <summary>
        /// for editor
        /// </summary>
        public StringContent keyContentDataSet;
        public string selectedKeyContent;
        public int previousSelectedIndex;

        public string EventKey { get { return selectedKeyContent; } }
        public string DispatcherName { get { return this.gameObject.name; } }
        public void DispatchEvent()
        {
            CosmosEntry.EventManager.DispatchEvent(EventKey, this, null);
        }
        /// <summary>
        /// 注销事件，事件派发者注销这个Key所持有的所有事件
        /// </summary>
        public void DeregisterEvent()
        {
            CosmosEntry.EventManager.DeregisterEvent(EventKey);
        }
    }
}