using System.Collections.Generic;

namespace Cosmos.Resource.Comparer
{
    public class ResourceComparer
    {
        /// <summary>
        /// 比较两个文件清单的差异；
        /// </summary>
        /// <param name="sourceManifest"></param>
        /// <param name="newManifest"></param>
        public void CompareManifest(ResourceManifest sourceManifest, ResourceManifest newManifest)
        {
            List< ResourceCompareInfo >compareInfos=new List<ResourceCompareInfo>();
            foreach (var bundleBuildInfo in newManifest.ResourceBundleBuildInfoDict)
            {
                if (!sourceManifest.ResourceBundleBuildInfoDict.ContainsKey(bundleBuildInfo.Key))
                {
                    //source不存在，则表示是new中最新的
                }
                else
                {
                    //source存在，校验hash是否一致
                }
            }
        }
    }
}
