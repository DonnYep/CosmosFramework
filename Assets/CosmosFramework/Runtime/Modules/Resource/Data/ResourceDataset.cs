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
        List<ResourceBundleInfo> resourceBundleInfoList;
        [SerializeField]
        List<string> resourceAvailableExtenisonList;
        [SerializeField]
        bool isChanged;
        /// <summary>
        /// 资源包数量；
        /// </summary>
        public int ResourceBundleCount { get { return resourceBundleInfoList.Count; } }
        /// <summary>
        /// 资源包；
        /// </summary>
        public List<ResourceBundleInfo> ResourceBundleInfoList
        {
            get
            {
                if (resourceBundleInfoList == null)
                    resourceBundleInfoList = new List<ResourceBundleInfo>();
                return resourceBundleInfoList;
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
            ResourceBundleInfoList.Clear();
        }
        /// <summary>
        /// 释放当前dataset；
        /// </summary>
        public void Dispose()
        {
            ResourceBundleInfoList.Clear();
            ResourceAvailableExtenisonList.Clear();
        }
    }
}
