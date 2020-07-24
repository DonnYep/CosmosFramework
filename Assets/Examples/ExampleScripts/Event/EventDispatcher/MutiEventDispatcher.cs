using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    [DisallowMultipleComponent]
    public class MutiEventDispatcher : MonoBehaviour
    {
        /// <summary>
        /// for editor
        /// </summary>
        public StringContent keyContentDataSet;
        public string[] selectedKeyContents;
        public int[] previousSelectedIndex;

        public string[] EventKeys { get { return selectedKeyContents; } }
        public string DispatcherName { get { return this.gameObject.name; } }
        public void DispatchEvent()
        {
        }
        /// <summary>
        /// 注销事件，事件派发者注销这个Key所持有的所有事件
        /// </summary>
        public void DeregisterEvent()
        {
        }
    }
}