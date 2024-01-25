using Cosmos.Resource.Compare;
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
            /// <summary>
            /// 合并文件清单，取并集
            ///  <para>(src ⋃ diff) </para>
            /// </summary>
            /// <param name="src">源文件清单</param>
            /// <param name="srcPath">源文件清单所在地址{SRC_PATH}/</param>
            /// <param name="diff">差异文件清单</param>
            /// <param name="diffPath">差异文件清单所在地址{DIFF_PATH}/</param>
            /// <returns>合并的文件清单</returns>
            public static ResourceMergedManifest MergeManifestByUnion(ResourceManifest src, string srcPath, ResourceManifest diff, string diffPath)
            {
                //union
                var mergedManifest = new ResourceMergedManifest();
                var srcBundleBuildInfoDict = src.ResourceBundleBuildInfoDict.Values.ToDictionary(info => info.ResourceBundle.BundlePath);
                var diffBundleBuildInfoDict = diff.ResourceBundleBuildInfoDict.Values.ToDictionary(info => info.ResourceBundle.BundlePath);
                var mergedBundleList = new List<ResourceMergedBundleBuildInfo>();
                foreach (var srcBundle in srcBundleBuildInfoDict.Values)
                {
                    var srcBundlePath = srcBundle.ResourceBundle.BundlePath;
                    if (diffBundleBuildInfoDict.TryGetValue(srcBundlePath, out var diffBundle))
                    {
                        //在diff中存在，比较bundlekey
                        var equal = diffBundle.BundleHash == srcBundle.BundleHash;
                        var path = string.Empty;
                        //hash相同表示diff存在且内容无变化，使用原始地址srcPath
                        //hash不相同表示diff存在但内容发生变更，使用变更地址diffPath
                        if (equal)
                        {
                            path = srcPath;
                        }
                        else
                        {
                            path = diffPath;
                        }
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            Path = path,
                            ResourceBundleBuildInfo = diffBundle,
                            Offset = diff.BundleOffset
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                    else
                    {
                        //diff不存在，则使用src的信息
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            Path = srcPath,
                            ResourceBundleBuildInfo = srcBundle,
                            Offset = src.BundleOffset
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                }
                foreach (var diffBundle in diffBundleBuildInfoDict.Values)
                {
                    var diffBundlePath = diffBundle.ResourceBundle.BundlePath;
                    if (!srcBundleBuildInfoDict.TryGetValue(diffBundlePath, out var srcBundle))
                    {
                        //src中不存在，diff中存在，表示新增，使用diffPath
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            Path = diffPath,
                            ResourceBundleBuildInfo = diffBundle
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                }
                mergedManifest.BuildVersion = mergedManifest.BuildVersion;
                mergedManifest.BuildHash = mergedManifest.BuildHash;
                mergedManifest.MergedBundleBuildInfoList = mergedBundleList;
                return mergedManifest;
            }
            /// <summary>
            /// 合并文件清单 
            /// <para> (src ⋂ diff) ⋃ (diff - (src ⋂ diff) )</para>
            /// <para>src与diff中同时存在且hash相同，表示为未变更，bundle地址使用srcPath</para>
            /// <para>src与diff中同时存在但hash不同，表示为变更，bundle地址使用diffPath</para>
            /// <para>src中存在但diff中不存在，表示为过期，忽略bundle</para>
            /// <para>src中不存在但diff中存在，表示为新增，bundle地址使用diffPath</para>
            /// </summary>
            /// <param name="src">源文件清单</param>
            /// <param name="srcPath">源文件清单所在地址{SRC_PATH}/</param>
            /// <param name="diff">差异文件清单</param>
            /// <param name="diffPath">差异文件清单所在地址{DIFF_PATH}/</param>
            /// <returns>合并的文件清单</returns>
            public static ResourceMergedManifest MergeManifest(ResourceManifest src, string srcPath, ResourceManifest diff, string diffPath)
            {
                //intersection
                var mergedManifest = new ResourceMergedManifest();
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
                        var path = string.Empty;
                        //hash相同表示diff存在且内容无变化，使用原始地址srcPath
                        //hash不相同表示diff存在但内容发生变更，使用变更地址diffPath
                        if (equal)
                            path = srcPath;
                        else
                            path = diffPath;
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            Path = path,
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
                        //src中不存在，diff中存在，表示新增，使用diffPath
                        var mergeInfo = new ResourceMergedBundleBuildInfo
                        {
                            Path = diffPath,
                            ResourceBundleBuildInfo = diffBundle
                        };
                        mergedBundleList.Add(mergeInfo);
                    }
                }
                mergedManifest.BuildVersion = mergedManifest.BuildVersion;
                mergedManifest.BuildHash = mergedManifest.BuildHash;
                mergedManifest.MergedBundleBuildInfoList = mergedBundleList;
                return mergedManifest;
            }
            #endregion

            /// <summary>
            ///  通过bundleKey比较两个文件清单的差异
            /// </summary>
            /// <param name="sourceManifest">原始的文件</param>
            /// <param name="comparisonManifest">用于比较的文件</param>
            /// <param name="result">比较结果</param>
            public static void CompareManifestByBundleKey(ResourceManifest sourceManifest, ResourceManifest comparisonManifest, out ResourceManifestCompareResult result)
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
                    var bundleName = srcBundleInfo.ResourceBundle.BundleName;
                    var bundleKey = srcBundleInfo.ResourceBundle.BundleKey;

                    var info = new ResourceManifestCompareInfo(bundleName, bundleKey, srcBundleInfo.BundleSize, srcBundleInfo.BundleHash);
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
                        var bundleName = cmpBundleInfo.ResourceBundle.BundleName;
                        var bundleKey = cmpBundleInfo.ResourceBundle.BundleKey;
                        newlyAdded.Add(new ResourceManifestCompareInfo(bundleName, bundleKey, cmpBundleInfo.BundleSize, cmpBundleInfo.BundleHash));
                    }
                }
                result.ChangedInfos = changed.ToArray();
                result.NewlyAddedInfos = newlyAdded.ToArray();
                result.ExpiredInfos = expired.ToArray();
                result.UnchangedInfos = unchanged.ToArray();
            }
            /// <summary>
            ///  通过bundleName比较两个文件清单的差异
            /// </summary>
            /// <param name="sourceManifest">原始的文件</param>
            /// <param name="comparisonManifest">用于比较的文件</param>
            /// <param name="result">比较结果</param>
            public static void CompareManifestByBundleName(ResourceManifest sourceManifest, ResourceManifest comparisonManifest, out ResourceManifestCompareResult result)
            {
                result = new ResourceManifestCompareResult();
                List<ResourceManifestCompareInfo> expired = new List<ResourceManifestCompareInfo>();
                List<ResourceManifestCompareInfo> changed = new List<ResourceManifestCompareInfo>();
                List<ResourceManifestCompareInfo> newlyAdded = new List<ResourceManifestCompareInfo>();
                List<ResourceManifestCompareInfo> unchanged = new List<ResourceManifestCompareInfo>();

                var srcBundleDict = sourceManifest.ResourceBundleBuildInfoDict.Values.ToDictionary(b => b.ResourceBundle.BundleName);
                var cmpBundleDict = comparisonManifest.ResourceBundleBuildInfoDict.Values.ToDictionary(b => b.ResourceBundle.BundleName);

                //这里使用src的文件清单遍历comparison的文件清单;
                foreach (var srcBundleBuildInfoKeyValue in srcBundleDict)
                {
                    var srcBundleInfo = srcBundleBuildInfoKeyValue.Value;

                    var bundleName = srcBundleInfo.ResourceBundle.BundleName;
                    var bundleKey = srcBundleInfo.ResourceBundle.BundleKey;

                    var info = new ResourceManifestCompareInfo(bundleName, bundleKey, srcBundleInfo.BundleSize, srcBundleInfo.BundleHash);

                    if (!cmpBundleDict.TryGetValue(srcBundleBuildInfoKeyValue.Key, out var cmpBundleBuildInfo))
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
                foreach (var cmpBundleBuildInfoKeyValue in cmpBundleDict)
                {
                    var cmpBundleInfo = cmpBundleBuildInfoKeyValue.Value;
                    if (!srcBundleDict.ContainsKey(cmpBundleBuildInfoKeyValue.Key))
                    {
                        //source中不存在，表示为新增资源；
                        var bundleName = cmpBundleInfo.ResourceBundle.BundleName;
                        var bundleKey = cmpBundleInfo.ResourceBundle.BundleKey;
                        newlyAdded.Add(new ResourceManifestCompareInfo(bundleName, bundleKey, cmpBundleInfo.BundleSize, cmpBundleInfo.BundleHash));
                    }
                }
                result.ChangedInfos = changed.ToArray();
                result.NewlyAddedInfos = newlyAdded.ToArray();
                result.ExpiredInfos = expired.ToArray();
                result.UnchangedInfos = unchanged.ToArray();
            }
        }
    }
}
