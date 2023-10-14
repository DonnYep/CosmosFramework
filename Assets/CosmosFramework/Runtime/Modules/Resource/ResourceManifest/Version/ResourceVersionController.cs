using Cosmos.Resource.Compare;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    public class ResourceVersionController
    {
        /// <summary>
        /// clean local expired bundles by compare resut
        /// </summary>
        /// <param name="localPath">local path ,can not url</param>
        /// <param name="compareResult">compare result of the manifest</param>
        public static void CleanExpiredBundlesByCompareResult(string localPath, ResourceManifestCompareResult compareResult)
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
            Utility.IO.DeleteDirectoryFiles(localPath, fileNames);
        }
    }
}
