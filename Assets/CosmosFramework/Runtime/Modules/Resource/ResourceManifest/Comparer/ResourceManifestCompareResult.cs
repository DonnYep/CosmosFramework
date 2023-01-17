using System;
namespace Cosmos.Resource.Compare
{
    /// <summary>
    /// 文件清单校验结果；
    /// </summary>
    [Serializable]
    public class ResourceManifestCompareResult
    {
        /// <summary>
        /// 有改动的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] ChangedInfos;
        /// <summary>
        /// 新增的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] NewlyAddedInfos;
        /// <summary>
        /// 过期无效的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] ExpiredInfos;
        /// <summary>
        /// 未更改的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] UnchangedInfos;
    }
}
