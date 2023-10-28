using Cosmos.Resource.Compare;
using System.Collections.Generic;
using System.Linq;

namespace Cosmos.Resource
{
    public class ResourceVersionController
    {
        /// <summary>
        /// clean local expired bundles by compare resut
        /// </summary>
        /// <param name="path">local path ,can not url</param>
        /// <param name="compareResult">compare result of the manifest</param>
        public static void CleanExpiredBundlesByCompareResult(string path, ResourceManifestCompareResult compareResult)
        {
            var changedInfos = compareResult.ChangedInfos;
            var expiredInfos = compareResult.ExpiredInfos;
            HashSet<string> fileNames = new HashSet<string>();
            var changedLength = changedInfos.Length;
            for (int i = 0; i < changedLength; i++)
            {
                fileNames.Add(changedInfos[i].ResouceBundleKey);
            }
            var expiredLength = expiredInfos.Length;
            for (int i = 0; i < expiredLength; i++)
            {
                fileNames.Add(expiredInfos[i].ResouceBundleKey);
            }
            Utility.IO.DeleteDirectoryFiles(path, fileNames);
        }
        /// <summary>
        /// 比较并清理失效资产
        /// </summary>
        /// <param name="src">源文件清单</param>
        /// <param name="diff">差异文件清单</param>
        /// <param name="path">本地持久化地址</param>
        public static void CompareAndCleanInvalidAssets(ResourceManifest src, ResourceManifest diff, string path)
        {
            ResourceUtility.Manifest.CompareManifestByBundleName(src, diff, out var result);
            List<string> deleteNames = new List<string>();
            var changedNames = result.ChangedInfos.Select(r => r.ResouceBundleKey);
            var expiredNames = result.ExpiredInfos.Select(r => r.ResouceBundleKey);
            deleteNames.AddRange(changedNames);
            deleteNames.AddRange(expiredNames);
            Utility.IO.DeleteDirectoryFiles(path, deleteNames);
        }
    }
}
