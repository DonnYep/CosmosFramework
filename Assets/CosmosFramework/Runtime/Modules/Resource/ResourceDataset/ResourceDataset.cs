using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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
        Dictionary<string, ResourceBundleInfo> resourceBundleInfoDict;
        /// <summary>
        /// 资源包数量；
        /// </summary>
        public int ResourceBundleCount { get { return ResourceBundleInfoList.Count; } }
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
        public Dictionary<string, ResourceBundleInfo> ResourceBundleInfoDict
        {
            get
            {
                if (resourceBundleInfoDict == null)
                {
                    resourceBundleInfoDict = GetResourceBundleInfos().ToDictionary((b) => b.BundleName);
                }
                return resourceBundleInfoDict;
            }
        }
        public List<ResourceBundleInfo> GetResourceBundleInfos()
        {
            List<ResourceBundleInfo> infoList = new List<ResourceBundleInfo>();
            var length = ResourceBundleCount;
            for (int i = 0; i < length; i++)
            {
                var bundleInfo = ResourceBundleInfoList[i];
                infoList.Add(bundleInfo);
                GetResourceBundleInfoRecursive(bundleInfo, ref infoList);
            }
            return infoList;
        }
        public bool PeekResourceBundleInfo(string displayName, out ResourceBundleInfo bundleInfo)
        {
            return ResourceBundleInfoDict.TryGetValue(displayName, out bundleInfo);
        }
        public void RegenerateBundleInfoDict()
        {
            resourceBundleInfoDict = GetResourceBundleInfos().ToDictionary((b)=>b.BundleName);
        }
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
        void GetResourceBundleInfoRecursive(ResourceBundleInfo bundleInfo, ref List<ResourceBundleInfo> infoList)
        {
            var subBundleInfos = bundleInfo.ResourceSubBundleInfoList;
            var length = subBundleInfos.Count;
            for (int i = 0; i < length; i++)
            {
                var subBundleInfo = subBundleInfos[i];
                infoList.Add(subBundleInfo);
                GetResourceBundleInfoRecursive(subBundleInfo, ref infoList);
            }
        }
    }
}
