using System.Collections.Generic;

namespace Cosmos.Resource.Comparer
{
    public class ResourceManifestComparer
    {
        /// <summary>
        ///  比较两个文件清单的差异；
        /// </summary>
        /// <param name="sourceManifest"></param>
        /// <param name="comparisonManifest"></param>
        /// <param name="result"></param>
        public void CompareManifest(ResourceManifest sourceManifest, ResourceManifest comparisonManifest, out ResourceManifestCompareResult result)
        {
            result = null;
            List<ResourceManifestCompareInfo> compareInfos = new List<ResourceManifestCompareInfo>();
            foreach (var _srcBundleBuildInfo in sourceManifest.ResourceBundleBuildInfoDict)
            {
                if (!comparisonManifest.ResourceBundleBuildInfoDict.TryGetValue(_srcBundleBuildInfo.Key, out var _cmpBundleBuildInfo))
                {
                    //如果comparison中不存在，则加入到差异队列中，并标准此文件已经过期
                }
            }
            foreach (var cmpBundleBuildInfo in comparisonManifest.ResourceBundleBuildInfoDict)
            {
                if (!sourceManifest.ResourceBundleBuildInfoDict.TryGetValue(cmpBundleBuildInfo.Key, out var srcBundleBuildInfo))
                {
                    //source不存在，则表示是new中最新的，加入到差异队列中
                }
                else
                {
                    //source存在，校验hash是否一致
                    var srcHash = srcBundleBuildInfo.BundleHash;
                    var comparisonHash = cmpBundleBuildInfo.Value.BundleHash;
                    if (srcHash != comparisonHash)
                    {
                        //hash不一致，加入到差异队列中
                    }
                }
            }
        }
    }
}
