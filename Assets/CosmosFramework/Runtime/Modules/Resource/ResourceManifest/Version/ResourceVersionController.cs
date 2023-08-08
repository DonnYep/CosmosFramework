using Cosmos.Resource.Compare;
using System.Collections.Generic;

namespace Cosmos.Resource
{
    public class ResourceVersionController
    {
        public static void ClearLocalDiscrepantBundles(string path, ResourceManifestCompareResult compareResult)
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
    }
}
