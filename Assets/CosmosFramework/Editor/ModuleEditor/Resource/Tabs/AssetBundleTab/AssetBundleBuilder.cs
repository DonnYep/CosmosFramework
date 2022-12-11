using Cosmos.Resource;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// AB打包构建器
    /// </summary>
    public class AssetBundleBuilder
    {
        public void PrepareBuildAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset, ref ResourceManifest resourceManifest)
        {
            if (Directory.Exists(buildParams.AssetBundleBuildPath))
                Utility.IO.DeleteFolder(buildParams.AssetBundleBuildPath);
            Directory.CreateDirectory(buildParams.AssetBundleBuildPath);

            var assetBundleNameType = buildParams.AssetBundleNameType;

            var bundleInfos = dataset.ResourceBundleInfoList;
            var bundleInfoLength = bundleInfos.Count;

            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundleInfo = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundleInfo.BundlePath);
                //这里获取绝对ab绝对路径下，所有资源的bytes，生成唯一MD5 hash
                var path = Path.Combine(EditorUtil.ApplicationPath(), bundleInfo.BundlePath);
                var hash = ResourceUtility.CreateDirectoryMd5(path);
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
                        }
                        break;
                }
                var bundle = new ResourceBundle()
                {
                    BundleKey = bundleKey,
                    BundleName = bundleInfo.BundleName,
                    BundlePath = bundleInfo.BundlePath,
                };
                bundle.DependenBundleKeytList.AddRange(bundleInfo.DependenBundleKeytList);
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
                var bundleBuildInfo = new ResourceManifest.ResourceBundleBuildInfo()
                {
                    BundleHash = hash,
                    ResourceBundle = bundle,
                    BundleSize = 0
                };
                //这里存储hash与bundle，打包出来的包体长度在下一个流程处理
                resourceManifest.ResourceBundleBuildInfoDict.Add(bundleInfo.BundleName, bundleBuildInfo);
            }
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundle = bundleInfos[i];
                bundle.DependenBundleKeytList.Clear();
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                bundle.DependenBundleKeytList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
        }
        public void ProcessAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset, AssetBundleManifest unityManifest, ref ResourceManifest resourceManifest)
        {
            Dictionary<string, ResourceBundleInfo> bundleKeyDict = null;
            if (buildParams.AssetBundleNameType == AssetBundleNameType.HashInstead)
                bundleKeyDict = dataset.ResourceBundleInfoList.ToDictionary(bundle => bundle.BundleKey);
            var bundleKeys = unityManifest.GetAllAssetBundles();
            var bundleKeyLength = bundleKeys.Length;
            for (int i = 0; i < bundleKeyLength; i++)
            {
                var bundleKey = bundleKeys[i];
                var bundlePath = Path.Combine(buildParams.AssetBundleBuildPath, bundleKey);
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
                            if (bundleKeyDict.TryGetValue(bundleKey, out var bundleInfo))
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
            //这段生成resourceManifest.json文件
            var encryptionKey = buildParams.BuildIedAssetsEncryptionKey;
            var encrypt = buildParams.BuildedAssetsEncryption;
            resourceManifest.BuildVersion = buildParams.BuildVersion;
            var manifestJson = EditorUtil.Json.ToJson(resourceManifest);
            var manifestContext = manifestJson;
            if (encrypt)
            {
                var key = ResourceUtility.GenerateBytesAESKey(encryptionKey);
                manifestContext = Utility.Encryption.AESEncryptStringToString(manifestJson, key);
            }
            Utility.IO.WriteTextFile(buildParams.AssetBundleBuildPath, ResourceConstants.RESOURCE_MANIFEST, manifestContext);

            //删除生成文对应的主manifest文件
            var buildVersionPath = Path.Combine(buildParams.AssetBundleBuildPath, buildParams.BuildVersion);
            var buildVersionManifestPath = Utility.Text.Append(buildVersionPath, ".manifest");
            Utility.IO.DeleteFile(buildVersionPath);
            Utility.IO.DeleteFile(buildVersionManifestPath);

            var bundleInfos = dataset.ResourceBundleInfoList;
            var bundleInfoLength = bundleInfos.Count;
            for (int i = 0; i < bundleInfoLength; i++)
            {
                var bundle = bundleInfos[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = string.Empty;
            }

            if (buildParams.CopyToStreamingAssets)
            {
                string streamingAssetPath = string.Empty;
                if (buildParams.UseStreamingAssetsRelativePath)
                    streamingAssetPath = Path.Combine(Application.streamingAssetsPath, buildParams.StreamingAssetsRelativePath);
                else
                    streamingAssetPath = Application.streamingAssetsPath;
                var buildPath = buildParams.AssetBundleBuildPath;
                if (Directory.Exists(buildPath))
                {
                    Utility.IO.CopyDirectory(buildPath, streamingAssetPath);
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
    }
}
