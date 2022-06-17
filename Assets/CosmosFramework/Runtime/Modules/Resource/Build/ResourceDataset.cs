using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Resource
{
    /// <summary>
    /// AssetDatabase 模式下资源寻址数据对象
    /// </summary>
    public class ResourceDataset : ScriptableObject
    {
        [SerializeField]
        List<ResourceObject> resourceObjectList;
        List<ResourceBundle> resourceBundleList;
        /// <summary>
        /// 资源对象数量；
        /// </summary>
        public int ResourceObjectCount { get { return ResourceObjectList.Count; } }
        /// <summary>
        /// 资源包数量；
        /// </summary>
        public int ResourceBundleCount { get { return resourceBundleList.Count; } }
        /// <summary>
        /// 资源对象列表；
        /// </summary>
        public List<ResourceObject> ResourceObjectList
        {
            get
            {
                if (resourceObjectList == null)
                    resourceObjectList = new List<ResourceObject>();
                return resourceObjectList;
            }
        }
        /// <summary>
        /// 资源包；
        /// </summary>
        public List<ResourceBundle> ResourceBundleList
        {
            get
            {
                if (resourceBundleList == null)
                    resourceBundleList = new List<ResourceBundle>();
                return resourceBundleList;
            }
        }
    }
}
