using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos.Resource
{
    /// <summary>
    /// AssetDatabase 模式下资源寻址数据对象
    /// </summary>
    public class ResourceDataset : ScriptableObject, IDisposable
    {
        [SerializeField]
        List<ResourceObject> resourceObjectList;
        [SerializeField]
        List<ResourceBundle> resourceBundleList;
        [SerializeField]
        List<string> resourceAvailableExtenisonList;
        [SerializeField]
        bool isChanged;
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
        /// <summary>
        /// 可识别的资源后缀列表；
        /// </summary>
        public List<string> ResourceAvailableExtenisonList
        {
            get
            {
                if (resourceAvailableExtenisonList == null)
                    resourceAvailableExtenisonList = new List<string>();
                return resourceAvailableExtenisonList;
            }
        }
        /// <summary>
        /// 是否做出了修改
        /// </summary>
        public bool IsChanged { get { return isChanged; } set { isChanged = value; } }
        /// <summary>
        /// 清空资源包与资源实体；
        /// </summary>
        public void Clear()
        {
            ResourceBundleList.Clear();
            ResourceObjectList.Clear();
        }
        /// <summary>
        /// 释放当前dataset；
        /// </summary>
        public void Dispose()
        {
            ResourceBundleList.Clear();
            ResourceObjectList.Clear();
            ResourceAvailableExtenisonList.Clear();
        }
    }
}
