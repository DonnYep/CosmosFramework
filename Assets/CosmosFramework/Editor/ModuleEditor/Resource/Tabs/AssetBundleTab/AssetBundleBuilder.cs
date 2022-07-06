using Cosmos.Resource;
using UnityEditor;
using UnityEngine;
using System.IO;
namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// AB打包构建器
    /// </summary>
    public class AssetBundleBuilder
    {
        public void PrepareBuildAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset)
        {
            if (Directory.Exists(buildParams.AssetBundleBuildPath))
                Utility.IO.DeleteFolder(buildParams.AssetBundleBuildPath);
            Directory.CreateDirectory(buildParams.AssetBundleBuildPath);
            var encryptionKey = buildParams.BuildIedAssetsEncryptionKey;
            var encrypt = buildParams.BuildedAssetsEncryption;
            var nameType = buildParams.BuildedAssetNameType;

            var bundles = dataset.ResourceBundleList;
            var bundleLength = bundles.Count;
            var resourceManifest = new ResourceManifest();

            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                var bundleName = string.Empty;
                switch (nameType)
                {
                    case BuildedAssetNameType.DefaultName:
                        {
                            bundleName = bundle.BundleName;
                        }
                        break;
                    case BuildedAssetNameType.HashInstead:
                        {
                            //这里获取绝对ab绝对路径下，所有资源的bytes，生成唯一MD5 hash
                            var path = Path.Combine(EditorUtil.ApplicationPath(), bundle.BundlePath);
                            var hash = ResourceUtility.CreateDirectoryMd5(path);
                            bundleName = hash;
                        }
                        break;
                }
                importer.assetBundleName = bundleName;
                resourceManifest.BundleManifestDict.Add(bundleName, bundle);
            }
            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                bundle.DependList.Clear();
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                bundle.DependList.AddRange(AssetDatabase.GetAssetBundleDependencies(importer.assetBundleName, true));
            }
            resourceManifest.BuildVersion = buildParams.BuildVersion;
            var manifestJson = EditorUtil.Json.ToJson(resourceManifest);
            var manifestContext = manifestJson;
            if (encrypt)
            {
                var key = ResourceUtility.GenerateBytesAESKey(encryptionKey);
                manifestContext = Utility.Encryption.AESEncryptStringToString(manifestJson, key);
            }
            Utility.IO.WriteTextFile(buildParams.AssetBundleBuildPath, ResourceConstants.RESOURCE_MANIFEST, manifestContext);
        }
        public void ProcessAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset, AssetBundleManifest manifest)
        {

            var bundleNames = manifest.GetAllAssetBundles();
            var bundleNameLength = bundleNames.Length;
            for (int i = 0; i < bundleNameLength; i++)
            {
                var bundlePath = Path.Combine(buildParams.AssetBundleBuildPath, bundleNames[i]);
                if (buildParams.AssetBundleEncryption)
                {
                    var bundleBytes = File.ReadAllBytes(bundlePath);

                    var offset = buildParams.AssetBundleOffsetValue;
                    Utility.IO.AppendAndWriteAllBytes(bundlePath, new byte[offset], bundleBytes);
                }
                var bundleManifestPath = Utility.Text.Append(bundlePath, ".manifest");
                Utility.IO.DeleteFile(bundleManifestPath);
            }

            //删除生成文对应的主manifest文件
            var buildVersionPath = Path.Combine(buildParams.AssetBundleBuildPath, buildParams.BuildVersion);
            var buildVersionManifestPath = Utility.Text.Append(buildVersionPath, ".manifest");
            Utility.IO.DeleteFile(buildVersionPath);
            Utility.IO.DeleteFile(buildVersionManifestPath);

            var bundles = dataset.ResourceBundleList;
            var bundleLength = bundles.Count;
            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                importer.assetBundleName = string.Empty;
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            System.GC.Collect();
        }
    }
}
