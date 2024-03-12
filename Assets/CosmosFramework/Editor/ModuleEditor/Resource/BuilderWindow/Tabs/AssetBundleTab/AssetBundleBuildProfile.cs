using UnityEngine;
namespace Cosmos.Editor.Resource
{
    public class AssetBundleBuildProfile : ScriptableObject
    {
        /// <summary>
        /// 打包数据
        /// </summary>
        [SerializeField] AssetBundleBuildProfileData assetBundleBuildProfileData;
        public AssetBundleBuildProfileData AssetBundleBuildProfileData
        {
            get
            {
                if (assetBundleBuildProfileData == null)
                    assetBundleBuildProfileData = new AssetBundleBuildProfileData();
                return assetBundleBuildProfileData;
            }
            set { assetBundleBuildProfileData = value; }
        }
        public ResourceBuildParams GetBuildParams()
        {
            var buildAssetBundleOptions = ResourceEditorUtility.Builder.GetBuildAssetBundleOptions(AssetBundleBuildProfileData.AssetBundleCompressType,
                AssetBundleBuildProfileData.DisableWriteTypeTree,
                AssetBundleBuildProfileData.DeterministicAssetBundle,
                AssetBundleBuildProfileData.ForceRebuildAssetBundle,
                AssetBundleBuildProfileData.IgnoreTypeTreeChanges);
            var buildParams = new ResourceBuildParams()
            {
                UseProjectRelativeBuildPath = AssetBundleBuildProfileData.UseProjectRelativeBuildPath,
                ProjectRelativeBuildPath = AssetBundleBuildProfileData.ProjectRelativeBuildPath,
                AssetBundleAbsoluteBuildPath = AssetBundleBuildProfileData.AssetBundleAbsoluteBuildPath,
                AssetBundleEncryption = AssetBundleBuildProfileData.AssetBundleEncryption,
                AssetBundleOffsetValue = AssetBundleBuildProfileData.AssetBundleOffsetValue,
                BuildAssetBundleOptions = buildAssetBundleOptions,
                AssetBundleNameType = AssetBundleBuildProfileData.AssetBundleNameType,
                EncryptManifest = AssetBundleBuildProfileData.EncryptManifest,
                ManifestEncryptionKey = AssetBundleBuildProfileData.ManifestEncryptionKey,
                BuildTarget = AssetBundleBuildProfileData.BuildTarget,
                ResourceBuildType = AssetBundleBuildProfileData.ResourceBuildType,
                BuildVersion = AssetBundleBuildProfileData.BuildVersion,
                InternalBuildVersion = AssetBundleBuildProfileData.InternalBuildVersion,
                CopyToStreamingAssets = AssetBundleBuildProfileData.CopyToStreamingAssets,
                UseStreamingAssetsRelativePath = AssetBundleBuildProfileData.UseStreamingAssetsRelativePath,
                StreamingAssetsRelativePath = AssetBundleBuildProfileData.StreamingAssetsRelativePath,
                BuildDetailOutputPath = AssetBundleBuildProfileData.BuildDetailOutputPath,
                ClearStreamingAssetsDestinationPath = AssetBundleBuildProfileData.ClearStreamingAssetsDestinationPath,
                ForceRemoveAllAssetBundleNames = AssetBundleBuildProfileData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = AssetBundleBuildProfileData.BuildHandlerName,
                AssetBundleExtension = AssetBundleBuildProfileData.UseAssetBundleExtension == true ? AssetBundleBuildProfileData.AssetBundleExtension : string.Empty
            };
            return buildParams;
        }
        public void Reset()
        {
            assetBundleBuildProfileData = new AssetBundleBuildProfileData();
        }
    }
}
