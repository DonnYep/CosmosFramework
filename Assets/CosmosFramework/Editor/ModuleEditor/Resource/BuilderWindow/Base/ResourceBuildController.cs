using Cosmos.Resource;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// AB打包构建器
    /// </summary>
    public class ResourceBuildController
    {
        /// <summary>
        /// 构建资源寻址数据
        /// </summary>
        /// <param name="dataset">资源寻址数据</param>
        public static void BuildDataset(ResourceDataset dataset)
        {
            if (dataset == null)
                return;
            var bundleInfos = dataset.ResourceBundleInfoList;
            var extensions = dataset.ResourceAvailableExtenisonList;
            var lowerExtensions = extensions.Select(s => s.ToLower()).ToArray();
            extensions.Clear();
            extensions.AddRange(lowerExtensions);
            var bundleInfoLength = bundleInfos.Count;

            List<ResourceBundleInfo> invalidBundleInfos = new List<ResourceBundleInfo>();

            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var bundlePath = bundleInfo.BundlePath;
                if (!AssetDatabase.IsValidFolder(bundlePath))
                {
                    invalidBundleInfos.Add(bundleInfo);
                    continue;
                }
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = bundleInfo.BundleName;

                var files = Utility.IO.GetAllFiles(bundlePath);
                var fileLength = files.Length;
                bundleInfo.ResourceObjectInfoList.Clear();
                for (int j = 0; j < fileLength; j++)
                {
                    var srcFilePath = files[j].Replace("\\", "/");
                    var srcFileExt = Path.GetExtension(srcFilePath);
                    var lowerFileExt = srcFileExt.ToLower();
                    if (extensions.Contains(lowerFileExt))
                    {
                        //统一使用小写的文件后缀名
                        var lowerExtFilePath = srcFilePath.Replace(srcFileExt, lowerFileExt);

                        var resourceObjectInfo = new ResourceObjectInfo()
                        {
                            BundleName = bundleInfo.BundleName,
                            Extension = lowerFileExt,
                            ObjectName = Path.GetFileNameWithoutExtension(lowerExtFilePath),
                            ObjectPath = lowerExtFilePath,
                            ObjectSize = EditorUtil.GetAssetFileSizeLength(lowerExtFilePath),
                            ObjectFormatBytes = EditorUtil.GetAssetFileSize(lowerExtFilePath),
                        };
                        resourceObjectInfo.ObjectVaild = AssetDatabase.LoadMainAssetAtPath(resourceObjectInfo.ObjectPath) != null;
                        bundleInfo.ResourceObjectInfoList.Add(resourceObjectInfo);
                    }
                }
                long bundleSize = EditorUtil.GetUnityDirectorySize(bundlePath, dataset.ResourceAvailableExtenisonList);
                bundleInfo.BundleSize = bundleSize;
                bundleInfo.BundleKey = bundleInfo.BundleName;
                bundleInfo.BundleFormatBytes = EditorUtility.FormatBytes(bundleSize);
            }
            for (int i = 0; i < invalidBundleInfos.Count; i++)
            {
                bundleInfos.Remove(invalidBundleInfos[i]);
            }
            var bundleDict = bundleInfos.ToDictionary(d => d.BundleKey);

            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                bundleInfo.BundleDependencies.Clear();
                var dependencies = AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true);
                var dependenciesLength = dependencies.Length;
                for (int j = 0; j < dependenciesLength; j++)
                {
                    var dependency = dependencies[j];
                    if (bundleDict.TryGetValue(dependency, out var resourceBundleInfo))
                    {
                        var bundleDependency = new ResourceBundleDependency()
                        {
                            BundleKey = resourceBundleInfo.BundleKey,
                            BundleName = resourceBundleInfo.BundleName
                        };
                        bundleInfo.BundleDependencies.Add(bundleDependency);
                    }
                }
            }
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            EditorUtility.SetDirty(dataset);
#if UNITY_2021_1_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(dataset);
#elif UNITY_2019_1_OR_NEWER
            AssetDatabase.SaveAssets();
#endif
            dataset.IsChanged = false;
        }
        /// <summary>
        /// 准备ab构建
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        /// <param name="bundleInfos">所有需要构建的包体信息</param>
        /// <param name="extensions">识别的文件后缀名</param>
        /// <param name="resourceManifest">框架的资源文件清单</param>
        public static void PrepareBuildAssetBundle(ResourceBuildParams buildParams, List<ResourceBundleInfo> bundleInfos, IList<string> extensions, ref ResourceManifest resourceManifest)
        {
            switch (buildParams.ResourceBuildType)
            {
                case ResourceBuildType.Full:
                    Utility.IO.EmptyFolder(buildParams.AssetBundleAbsoluteBuildPath);
                    break;
                case ResourceBuildType.Incremental:
                    Utility.IO.CreateFolder(buildParams.AssetBundleAbsoluteBuildPath);
                    break;
            }

            var assetBundleNameType = buildParams.AssetBundleNameType;

            var bundleInfoLength = bundleInfos.Count;
            var useAssetBundleExtension = !string.IsNullOrEmpty(buildParams.AssetBundleExtension);
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                //过滤空包。若文件夹被标记为bundle，且不包含内容，则unity会过滤。因此遵循unity的规范；
                if (bundleInfo.ResourceObjectInfoList.Count <= 0)
                    continue;
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                //这里获取绝对ab绝对路径下，所有资源的bytes，生成唯一MD5 hash
                var path = Path.Combine(EditorUtil.ApplicationPath(), bundleInfo.BundlePath);
                string hash = string.Empty;
                if (bundleInfo.Extract)
                {
                    hash = Utility.IO.GenerateFileMD5(path);
                }
                else
                {
                    hash = Utility.IO.GenerateDirectoryMD5(path, extensions);
                }
                var bundleKey = string.Empty;
                switch (assetBundleNameType)
                {
                    case AssetBundleNameType.DefaultName:
                        {
                            importer.assetBundleName = bundleInfo.BundleName;
                            bundleKey = bundleInfo.BundleName;
                        }
                        break;
                    case AssetBundleNameType.HashInstead:
                        {
                            importer.assetBundleName = hash;
                            bundleKey = hash;
                            bundleInfo.BundleKey = hash;
                        }
                        break;
                }
                string buildExtension = string.Empty;
                if (useAssetBundleExtension)
                {
                    importer.assetBundleVariant = buildParams.AssetBundleExtension;
                    buildExtension = buildParams.AssetBundleExtension;
                }

                var bundle = new ResourceBundle()
                {
                    BundleKey = bundleKey,
                    BundleName = bundleInfo.BundleName,
                    BundlePath = bundleInfo.BundlePath,
                };
                var objectInfoList = bundleInfo.ResourceObjectInfoList;
                var objectInfoLength = objectInfoList.Count;
                for (int j = 0; j < objectInfoLength; j++)
                {
                    var objectInfo = objectInfoList[j];
                    var resourceObject = new ResourceObject()
                    {
                        ObjectName = objectInfo.ObjectName,
                        ObjectPath = objectInfo.ObjectPath,
                        BundleName = objectInfo.BundleName,
                        Extension = objectInfo.Extension
                    };
                    bundle.ResourceObjectList.Add(resourceObject);
                }
                var bundleBuildInfo = new ResourceBundleBuildInfo()
                {
                    BundleHash = hash,
                    ResourceBundle = bundle,
                    BundleSize = 0,
                    BudleExtension = buildExtension
                };
                //这里存储hash与bundle，打包出来的包体长度在下一个流程处理
                resourceManifest.ResourceBundleBuildInfoDict.Add(bundleInfo.BundleName, bundleBuildInfo);
            }
            //refresh assetbundle
            AssetDatabase.Refresh();
            var bundleDict = bundleInfos.ToDictionary(d => d.BundleKey);
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                bundleInfo.BundleDependencies.Clear();
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                var dependencies = AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true);
                var dependenciesLength = dependencies.Length;
                for (int j = 0; j < dependenciesLength; j++)
                {
                    var dependency = dependencies[j];
                    if (bundleDict.TryGetValue(dependency, out var resourceBundleInfo))
                    {
                        var bundleDependency = new ResourceBundleDependency()
                        {
                            BundleKey = resourceBundleInfo.BundleKey,
                            BundleName = resourceBundleInfo.BundleName
                        };
                        bundleInfo.BundleDependencies.Add(bundleDependency);
                    }
                }
                if (resourceManifest.ResourceBundleBuildInfoDict.TryGetValue(bundleInfo.BundleName, out var bundleBuildInfo))
                {
                    bundleBuildInfo.ResourceBundle.BundleDependencies.Clear();
                    bundleBuildInfo.ResourceBundle.BundleDependencies.AddRange(bundleInfo.BundleDependencies);
                }
            }
        }
        /// <summary>
        /// 处理ab包
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        /// <param name="bundleInfos">所有需要构建的包体信息</param>
        /// <param name="unityManifest">unity的资源文件清单</param>
        /// <param name="resourceManifest">框架的资源文件清单</param>
        public static void ProcessAssetBundle(ResourceBuildParams buildParams, List<ResourceBundleInfo> bundleInfos, AssetBundleManifest unityManifest, ref ResourceManifest resourceManifest)
        {
            Dictionary<string, ResourceBundleInfo> bundleDict = null;
            bundleDict = bundleInfos.ToDictionary(bundle => bundle.BundleKey);
            var bundleKeys = unityManifest.GetAllAssetBundles();
            var bundleKeyLength = bundleKeys.Length;
            for (int i = 0; i < bundleKeyLength; i++)
            {
                var bundleKey = bundleKeys[i];
                var bundlePath = Path.Combine(buildParams.AssetBundleAbsoluteBuildPath, bundleKey);
                long bundleSize = 0;
                if (buildParams.AssetBundleEncryption)
                {
                    var bundleBytes = File.ReadAllBytes(bundlePath);
                    var offset = buildParams.AssetBundleOffsetValue;
                    bundleSize = Utility.IO.AppendAndWriteAllBytes(bundlePath, new byte[offset], bundleBytes);
                }
                else
                {
                    var bundleBytes = File.ReadAllBytes(bundlePath);
                    bundleSize = bundleBytes.LongLength;
                }
                var bundleName = string.Empty;
                switch (buildParams.AssetBundleNameType)
                {
                    case AssetBundleNameType.DefaultName:
                        {
                            bundleName = bundleKey;
                        }
                        break;
                    case AssetBundleNameType.HashInstead:
                        {
                            if (bundleDict.TryGetValue(bundleKey, out var bundleInfo))
                                bundleName = bundleInfo.BundleName;
                        }
                        break;
                }
                if (resourceManifest.ResourceBundleBuildInfoDict.TryGetValue(bundleName, out var resourceBundleBuildInfo))
                {
                    //这里存储打包出来的AB长度
                    resourceBundleBuildInfo.BundleSize = bundleSize;
                }
                var bundleManifestPath = Utility.Text.Append(bundlePath, ".manifest");
                Utility.IO.DeleteFile(bundleManifestPath);
            }

            var bundleInfoLength = bundleInfos.Count;

            #region 还原dataset在editor环境下的依赖
            //这段还原dataset在editor模式的依赖，并还原bundleKey；
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                importer.assetBundleName = bundleInfo.BundleName;
                bundleInfo.BundleKey = bundleInfo.BundleName;
            }
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                bundleInfo.BundleDependencies.Clear();
                var dependencies = AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true);
                var dependenciesLength = dependencies.Length;
                for (int j = 0; j < dependenciesLength; j++)
                {
                    var dependency = dependencies[j];
                    if (bundleDict.TryGetValue(dependency, out var resourceBundleInfo))
                    {
                        var bundleDependency = new ResourceBundleDependency()
                        {
                            BundleKey = resourceBundleInfo.BundleKey,
                            BundleName = resourceBundleInfo.BundleName
                        };
                        bundleInfo.BundleDependencies.Add(bundleDependency);
                    }
                }
            }
            #endregion

            //refresh assetbundle
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
        /// <summary>
        /// 处理文件清单，如记录包体的信息，版本号等。
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        /// <param name="resourceManifest">框架的资源文件清单</param>
        public static void PorcessManifest(ResourceBuildParams buildParams, ref ResourceManifest resourceManifest)
        {
            //这段生成resourceManifest.json文件
            var encryptionKey = buildParams.ManifestEncryptionKey;
            var encrypt = buildParams.EncryptManifest;
            resourceManifest.BuildVersion = buildParams.BuildVersion;
            resourceManifest.BuildHash = Utility.Encryption.GUID();
            if (buildParams.AssetBundleEncryption)
                resourceManifest.BundleOffset = (ulong)buildParams.AssetBundleOffsetValue;
            var manifestJson = EditorUtil.Json.ToJson(resourceManifest);
            var manifestContext = manifestJson;
            if (encrypt)
            {
                var key = ResourceUtility.GenerateBytesAESKey(encryptionKey);
                manifestContext = Utility.Encryption.AESEncryptStringToString(manifestJson, key);
            }
            Utility.IO.WriteTextFile(buildParams.AssetBundleAbsoluteBuildPath, ResourceConstants.RESOURCE_MANIFEST, manifestContext);

            //删除生成文对应的主manifest文件
            string buildVersionPath = string.Empty;
            switch (buildParams.ResourceBuildType)
            {
                case ResourceBuildType.Full:
                    buildVersionPath = Path.Combine(buildParams.AssetBundleAbsoluteBuildPath, $"{buildParams.BuildVersion}_{buildParams.InternalBuildVersion}");
                    break;
                case ResourceBuildType.Incremental:
                    buildVersionPath = Path.Combine(buildParams.AssetBundleAbsoluteBuildPath, buildParams.BuildVersion);
                    break;
            }
            var buildVersionManifestPath = Utility.Text.Append(buildVersionPath, ".manifest");
            Utility.IO.DeleteFile(buildVersionPath);
            Utility.IO.DeleteFile(buildVersionManifestPath);
        }
        /// <summary>
        /// 处理构建完成后的选项。例如将构建的资源拷贝到工程内等。
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        public static void BuildDoneOption(ResourceBuildParams buildParams)
        {
            if (buildParams.CopyToStreamingAssets)
            {
                string streamingAssetPath = string.Empty;
                if (buildParams.UseStreamingAssetsRelativePath)
                    streamingAssetPath = Path.Combine(Application.streamingAssetsPath, buildParams.StreamingAssetsRelativePath);
                else
                    streamingAssetPath = Application.streamingAssetsPath;
                var buildPath = buildParams.AssetBundleAbsoluteBuildPath;
                if (buildParams.ClearStreamingAssetsDestinationPath)
                {
                    Utility.IO.EmptyFolder(streamingAssetPath);
                }
                if (Directory.Exists(buildPath))
                {
                    Utility.IO.CopyDirectory(buildPath, streamingAssetPath);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
        /// <summary>
        /// 比较增量构建的缓存，根据缓存信息生成比较结果。
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        /// <param name="bundleInfos">所有需要构建的包体信息</param>
        /// <param name="extensions">识别的文件后缀名</param>
        /// <param name="cacheCompareResult">缓存比较结果</param>
        public static void CompareIncrementalBuildCache(ResourceBuildParams buildParams, List<ResourceBundleInfo> bundleInfos, IList<string> extensions, out ResourceBuildCacheCompareResult cacheCompareResult)
        {
            cacheCompareResult = new ResourceBuildCacheCompareResult();
            ResourceBuildCache buildCache = default;
            try
            {
                var buildCacheWritePath = Path.Combine(buildParams.BuildDetailOutputPath, ResourceEditorConstants.RESOURCE_BUILD_CACHE);
                var cacheJson = Utility.IO.ReadTextFileContent(buildCacheWritePath);
                buildCache = EditorUtil.Json.ToObject<ResourceBuildCache>(cacheJson);
            }
            catch
            {
                buildCache = new ResourceBuildCache()
                {
                    BundleCacheInfoList = new List<ResourceBundleCacheInfo>()
                };
            }

            var newlyAdded = new List<ResourceBundleCacheInfo>();
            var changed = new List<ResourceBundleCacheInfo>();
            var expired = new List<ResourceBundleCacheInfo>();
            var unchanged = new List<ResourceBundleCacheInfo>();

            var newBundleCacheInfoList = new List<ResourceBundleCacheInfo>();
            var srcBundleCacheInfoList = buildCache.BundleCacheInfoList;
            var srcBundleCacheInfoDict = srcBundleCacheInfoList.ToDictionary(b => b.BundlePath);
            var cmpBundleCacheInfoList = new List<ResourceBundleCacheInfo>();
            foreach (var bundleInfo in bundleInfos)
            {
                if (bundleInfo.ResourceObjectInfoList.Count <= 0)
                    continue;
                var bundlePath = bundleInfo.BundlePath;
                var bundleName = bundleInfo.BundleName;
                var path = Path.Combine(EditorUtil.ApplicationPath(), bundlePath);
                var hash = Utility.IO.GenerateDirectoryMD5(path, extensions);
                var assetNames = bundleInfo.ResourceObjectInfoList.Select(obj => obj.ObjectPath).ToArray();
                var cmpCacheInfo = new ResourceBundleCacheInfo()
                {
                    BundleName = bundleName,
                    BundlePath = bundlePath,
                    BundleHash = hash,
                    AssetNames = assetNames,
                };
                cmpBundleCacheInfoList.Add(cmpCacheInfo);
            }
            var cmpCacheInfoDict = cmpBundleCacheInfoList.ToDictionary(b => b.BundlePath);
            var nameType = buildParams.AssetBundleNameType;
            var abBuildPath = buildParams.AssetBundleAbsoluteBuildPath;
            foreach (var srcCacheInfo in srcBundleCacheInfoDict.Values)
            {
                var filePath = string.Empty;
                switch (nameType)
                {
                    case AssetBundleNameType.DefaultName:
                        filePath = Path.Combine(abBuildPath, srcCacheInfo.BundleName);
                        break;
                    case AssetBundleNameType.HashInstead:
                        filePath = Path.Combine(abBuildPath, srcCacheInfo.BundleHash);
                        break;
                }
                if (!cmpCacheInfoDict.TryGetValue(srcCacheInfo.BundlePath, out var cmpCacheInfo))
                {
                    //现有资源不存在，表示为过期
                    expired.Add(srcCacheInfo);
                    Utility.IO.DeleteFile(filePath);
                }
                else
                {
                    if (srcCacheInfo.BundleHash != cmpCacheInfo.BundleHash)
                    {
                        changed.Add(cmpCacheInfo);
                        Utility.IO.DeleteFile(filePath);
                    }
                    else
                    {
                        if (File.Exists(filePath))
                        {
                            unchanged.Add(cmpCacheInfo);
                        }
                        else
                        {
                            changed.Add(cmpCacheInfo);
                        }
                    }
                    newBundleCacheInfoList.Add(cmpCacheInfo);
                }
            }
            foreach (var cmpCacheInfo in cmpCacheInfoDict.Values)
            {
                if (!srcBundleCacheInfoDict.TryGetValue(cmpCacheInfo.BundlePath, out var srcCacheInfo))
                {
                    newlyAdded.Add(cmpCacheInfo);
                    newBundleCacheInfoList.Add(cmpCacheInfo);
                }
            }
            cacheCompareResult.NewlyAdded = newlyAdded.ToArray();
            cacheCompareResult.Changed = changed.ToArray();
            cacheCompareResult.Expired = expired.ToArray();
            cacheCompareResult.Unchanged = unchanged.ToArray();
            cacheCompareResult.BundleCacheInfoList = newBundleCacheInfoList;
        }
        /// <summary>
        /// 生成增量构建日子
        /// </summary>
        /// <param name="buildParams">构建参数</param>
        /// <param name="cacheCompareResult">比较结果数据</param>
        public static void GenerateIncrementalBuildLog(ResourceBuildParams buildParams, ResourceBuildCacheCompareResult cacheCompareResult)
        {
            var newBuildCache = new ResourceBuildCache()
            {
                BuildVerison = buildParams.BuildVersion,
                InternalBuildVerison = buildParams.InternalBuildVersion,
                BundleCacheInfoList = cacheCompareResult.BundleCacheInfoList
            };
            var cacheJson = EditorUtil.Json.ToJson(newBuildCache);
            var cachePath = Path.Combine(buildParams.BuildDetailOutputPath, ResourceEditorConstants.RESOURCE_BUILD_CACHE);
            Utility.IO.OverwriteTextFile(cachePath, cacheJson);

            var logJson = EditorUtil.Json.ToJson(cacheCompareResult);
            var logPath = Path.Combine(buildParams.BuildDetailOutputPath, ResourceEditorConstants.RESOURCE_BUILD_LOG);
            Utility.IO.OverwriteTextFile(logPath, logJson);
        }
        /// <summary>
        /// 还原ab的名称
        /// </summary>
        /// <param name="bundleInfos">所有需要构建的包体信息</param>
        public static void RevertAssetBundlesName(List<ResourceBundleInfo> bundleInfos)
        {
            var bundleInfoLength = bundleInfos.Count;
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundle = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
        /// <summary>
        /// 构建ab资源
        /// </summary>
        /// <param name="dataset">资源寻址数据</param>
        /// <param name="buildParams">构建参数</param>
        public static void BuildAssetBundle(ResourceDataset dataset, ResourceBuildParams buildParams)
        {
            if (dataset == null)
                return;
            var startTime = DateTime.Now;
            BuildDataset(dataset);
            ResourceManifest resourceManifest = new ResourceManifest();
            var bundleInfos = dataset.GetResourceBundleInfos();
            var extensions = dataset.ResourceAvailableExtenisonList.ToArray();
            PrepareBuildAssetBundle(buildParams, bundleInfos, extensions, ref resourceManifest);
            var resourceBuildHandler = Utility.Assembly.GetTypeInstance<IResourceBuildHandler>(buildParams.BuildHandlerName);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildPrepared(buildParams);
            }
            var unityManifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleAbsoluteBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
            ProcessAssetBundle(buildParams, bundleInfos, unityManifest, ref resourceManifest);
            PorcessManifest(buildParams, ref resourceManifest);
            BuildDoneOption(buildParams);
            if (resourceBuildHandler != null)
            {
                resourceBuildHandler.OnBuildComplete(buildParams);
            }
            RevertAssetBundlesName(bundleInfos);
            var endTime = DateTime.Now;
            var elapsedTime = endTime - startTime;
            EditorUtil.Debug.LogInfo($"Assetbundle build done , elapsed time {elapsedTime.Hours}h :{elapsedTime.Minutes}m :{elapsedTime.Seconds}s :{elapsedTime.Milliseconds}ms");
        }
    }
}
