using System;
namespace Cosmos.Resource.Comparer
{
    /// <summary>
    /// 文件清单校验结果；
    /// </summary>
    [Serializable]
    public class ResourceManifestCompareResult
    {
        /// <summary>
        /// 名称一致但是hash不同的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] ExpiredInfos;
        /// <summary>
        /// 新增的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] AddedInfos;
        /// <summary>
        /// 需要删除的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] RemovedInfos;
        /// <summary>
        /// 相同的文件；
        /// </summary>
        public ResourceManifestCompareInfo[] MatchedInfos;
    }
}
