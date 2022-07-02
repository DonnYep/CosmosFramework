using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var encrypt = !string.IsNullOrEmpty(encryptionKey);
            var nameType = buildParams.BuildedAssetNameType;

            var bundles = dataset.ResourceBundleList;
            var bundleLength = bundles.Count;
            for (int i = 0; i < bundleLength; i++)
            {
                var bundle = bundles[i];
                var importer = AssetImporter.GetAtPath(bundle.BundlePath);
                if (encrypt)
                {
                    switch (nameType)
                    {
                        case BuildedAssetNameType.DefaultName:
                            {
                                importer.assetBundleName = bundle.BundleName;
                            }
                            break;
                        case BuildedAssetNameType.HashInstead:
                            {
                                var encryptedName = Utility.Encryption.MD5Encrypt32(bundle.BundleName);
                                importer.assetBundleName = encryptedName;
                            }
                            break;
                    }
                }
            }
        }
        public void ProcessAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset, AssetBundleManifest manifest)
        {
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
