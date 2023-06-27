using System.Collections.Generic;

namespace Cosmos.Editor.Resource
{
    public class ResourceBuildCacheCompareResult
    {
        /// <summary>
        /// 新增的；
        /// </summary>
        public ResourceBundleCacheInfo[] NewlyAdded;
        /// <summary>
        /// 有改动的文件；
        /// </summary>
        public ResourceBundleCacheInfo[] Changed;
        /// <summary>
        /// 过期无效的文件；
        /// </summary>
        public ResourceBundleCacheInfo[] Expired;
        /// <summary>
        /// 未更改的文件；
        /// </summary>
        public ResourceBundleCacheInfo[] Unchanged;
        /// <summary>
        /// 文件缓存
        /// </summary>
        public List<ResourceBundleCacheInfo> BundleCacheInfoList;
    }
}
