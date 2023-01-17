using System.Collections.Generic;

namespace Cosmos.Resource.Compare
{
    public class ResourceManifestComparer
    {
        /// <summary>
        ///  比较两个文件清单的差异；
        /// </summary>
        /// <param name="sourceManifest">原始的文件</param>
        /// <param name="comparisonManifest">用于比较的文件</param>
        /// <param name="result">比较结果</param>
        public void CompareManifest(ResourceManifest sourceManifest, ResourceManifest comparisonManifest, out ResourceManifestCompareResult result)
        {
            result = new ResourceManifestCompareResult();
            List<ResourceManifestCompareInfo> expired = new List<ResourceManifestCompareInfo>();
            List<ResourceManifestCompareInfo> changed = new List<ResourceManifestCompareInfo>();
            List<ResourceManifestCompareInfo> newlyAdded = new List<ResourceManifestCompareInfo>();
            List<ResourceManifestCompareInfo> unchanged = new List<ResourceManifestCompareInfo>();
            //这里使用src的文件清单遍历comparison的文件清单;
            foreach (var srcBundleBuildInfoKeyValue in sourceManifest.ResourceBundleBuildInfoDict)
            {
                var srcBundleInfo = srcBundleBuildInfoKeyValue.Value;
                var info = new ResourceManifestCompareInfo(srcBundleInfo.ResourceBundle.BundleName, srcBundleInfo.ResourceBundle.BundleKey, srcBundleInfo.BundleSize, srcBundleInfo.BundleHash);

                if (!comparisonManifest.ResourceBundleBuildInfoDict.TryGetValue(srcBundleBuildInfoKeyValue.Key, out var cmpBundleBuildInfo))
                {
                    //如果comparison中不存在，表示资源已经过期，加入到移除的列表中；
                    expired.Add(info);
                }
                else
                {
                    //如果comparison中存在，则比较Hash
                    if (srcBundleInfo.BundleHash != cmpBundleBuildInfo.BundleHash)
                    {
                        //Hash不一致，表示需要更新；
                        changed.Add(info);
                    }
                    else
                    {
                        //Hash一致，无需更新；
                        unchanged.Add(info);
                    }
                }
            }
            foreach (var cmpBundleBuildInfoKeyValue in comparisonManifest.ResourceBundleBuildInfoDict)
            {
                var cmpBundleInfo = cmpBundleBuildInfoKeyValue.Value;
                if (!sourceManifest.ResourceBundleBuildInfoDict.ContainsKey(cmpBundleBuildInfoKeyValue.Key))
                {
                    //source中不存在，表示为新增资源；
                    newlyAdded.Add(new ResourceManifestCompareInfo(cmpBundleInfo.ResourceBundle.BundleName, cmpBundleInfo.ResourceBundle.BundleKey, cmpBundleInfo.BundleSize, cmpBundleInfo.BundleHash));
                }
            }
            result.ChangedInfos = changed.ToArray();
            result.NewlyAddedInfos= newlyAdded.ToArray();
            result.ExpiredInfos = expired.ToArray();
            result.UnchangedInfos = unchanged.ToArray();
        }
    }
}
