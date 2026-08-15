using System;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    /// <summary>
    /// 新资源管线构建参数
    /// </summary>
    [Serializable]
    public struct CFResourceBuildParams
    {
        /// <summary>
        /// AB打包到的平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// AB打包选项
        /// </summary>
        public BuildAssetBundleOptions BuildAssetBundleOptions;
        /// <summary>
        /// 输出根目录（工程相对路径）
        /// </summary>
        public string OutputRootPath;
        /// <summary>
        /// 构建版本
        /// </summary>
        public string BuildVersion;
        /// <summary>
        /// 清单加密密钥
        /// </summary>
        public string ManifestEncryptionKey;
        /// <summary>
        /// 是否加密清单
        /// </summary>
        public bool EncryptManifest;
        /// <summary>
        /// 拷贝到StreamingAssets
        /// </summary>
        public bool CopyToStreamingAssets;
        /// <summary>
        /// StreamingAssets相对路径
        /// </summary>
        public string StreamingAssetsRelativePath;
        /// <summary>
        /// 清空目标目录
        /// </summary>
        public bool ClearOutputPath;

        public static CFResourceBuildParams Default
        {
            get
            {
                return new CFResourceBuildParams()
                {
                    BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                    BuildAssetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression,
                    OutputRootPath = ResourceEditorConstants.DEFAULT_PACKAGE_OUTPUT_PATH,
                    BuildVersion = "0.0.1",
                    ManifestEncryptionKey = "CosmosBundlesKey",
                    EncryptManifest = false,
                    CopyToStreamingAssets = false,
                    StreamingAssetsRelativePath = ResourceEditorConstants.DEFAULT_PACKAGE_STREAMING_ASSETS_PATH,
                    ClearOutputPath = true,
                };
            }
        }
    }
}
