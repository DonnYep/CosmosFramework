using System.Collections.Generic;
using System.Linq;

namespace Cosmos.Resource
{
    public static partial class ResourceUtility
    {
        public static class Manifest
        {
            #region ResourceManifest
            public static string SerializeManifest(ResourceManifest manifest, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return SerializeManifest(manifest, keyBytes);
            }
            public static string SerializeManifest(ResourceManifest manifest, byte[] keyBytes)
            {
                var json = LitJson.JsonMapper.ToJson(manifest);
                var hasKey = keyBytes != null && keyBytes.Length > 0;
                string context = string.Empty;
                if (hasKey)
                    context = Utility.Encryption.AESEncryptStringToString(json, keyBytes);
                else
                    context = json;
                return context;
            }
            public static ResourceManifest DeserializeManifest(string srcContext, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return DeserializeManifest(srcContext, keyBytes);
            }
            public static ResourceManifest DeserializeManifest(string srcContext, byte[] keyBytes)
            {
                ResourceManifest manifest = null;
                try
                {
                    var hasKey = keyBytes != null && keyBytes.Length > 0;
                    var context = srcContext;
                    if (hasKey)
                    {
                        context = Utility.Encryption.AESDecryptStringToString(srcContext, keyBytes);
                    }
                    manifest = LitJson.JsonMapper.ToObject<ResourceManifest>(context);
                }
                catch { }
                return manifest;
            }
            #endregion

            #region ResourceMergedManifest
            public static string SerializeMergedManifest(ResourceMergedManifest mergedManifest, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return SerializeMergedManifest(mergedManifest, keyBytes);
            }
            public static string SerializeMergedManifest(ResourceMergedManifest mergedManifest, byte[] keyBytes)
            {
                var json = LitJson.JsonMapper.ToJson(mergedManifest);
                var hasKey = keyBytes != null && keyBytes.Length > 0;
                string context = string.Empty;
                if (hasKey)
                    context = Utility.Encryption.AESEncryptStringToString(json, keyBytes);
                else
                    context = json;
                return context;
            }
            public static ResourceMergedManifest DeserializeMergedManifest(string srcContext, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return DeserializeMergedManifest(srcContext, keyBytes);
            }
            public static ResourceMergedManifest DeserializeMergedManifest(string srcContext, byte[] keyBytes)
            {
                ResourceMergedManifest mergedManifest = null;
                try
                {
                    var hasKey = keyBytes != null && keyBytes.Length > 0;
                    var context = srcContext;
                    if (hasKey)
                    {
                        context = Utility.Encryption.AESDecryptStringToString(srcContext, keyBytes);
                    }
                    mergedManifest = LitJson.JsonMapper.ToObject<ResourceMergedManifest>(context);
                }
                catch { }
                return mergedManifest;
            }
            #endregion

            #region Merge
            public static void MergeManifest(ResourceManifest src, ResourceManifest diff, out ResourceMergedManifest mergedManifest)
            {
                //get union
                mergedManifest = new ResourceMergedManifest();
                var srcBundleBuildInfoDict = src.ResourceBundleBuildInfoDict.Values.ToDictionary(info => info.ResourceBundle.BundlePath);
                var diffBundleBuildInfoDict = diff.ResourceBundleBuildInfoDict.Values.ToDictionary(info => info.ResourceBundle.BundlePath);
                var mergedBundleList = new List<ResourceMergedBundleBuildInfo>();
                foreach (var srcBundle in srcBundleBuildInfoDict.Values)
                {
                    var srcBundlePath = srcBundle.ResourceBundle.BundlePath;
                    if (diffBundleBuildInfoDict.TryGetValue(srcBundlePath, out var diffBundle))
                    {
                        //在diffmanifest中存在，比较bundlekey
                        var equal = diffBundle.BundleHash == srcBundle.BundleHash;
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            //IsIncremental = !equal,
                            ResourceBundleBuildInfo = diffBundle
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                }
                foreach (var diffBundle in diffBundleBuildInfoDict.Values)
                {
                    var diffBundlePath = diffBundle.ResourceBundle.BundlePath;
                    if (!srcBundleBuildInfoDict.TryGetValue(diffBundlePath, out var srcBundle))
                    {
                        //src中不存在，diff中存在，则表示为新增的
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            //IsIncremental = true,
                            ResourceBundleBuildInfo = diffBundle
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                }
                mergedManifest.BuildVersion = mergedManifest.BuildVersion;
                mergedManifest.BuildHash = mergedManifest.BuildHash;
                mergedManifest.MergedBundleBuildInfoList = mergedBundleList;
            }
            #endregion
        }
    }
}
