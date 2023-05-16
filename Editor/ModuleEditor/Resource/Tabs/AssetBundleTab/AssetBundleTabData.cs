﻿using System;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    [Serializable]
    public class AssetBundleTabData
    {
        /// <summary>
        /// AB打包到的平台
        /// </summary>
        public BuildTarget BuildTarget;
        /// <summary>
        /// AB资源压缩类型；
        /// </summary>
        public AssetBundleCompressType AssetBundleCompressType;
        /// <summary>
        /// AB资源名称类型
        /// </summary>
        public AssetBundleNameType AssetBundleNameType;
        /// <summary>
        /// 构建handler的名称
        /// </summary>
        public string BuildHandlerName;
        /// <summary>
        /// 构建handler的序号
        /// </summary>
        public int BuildHandlerIndex;
        /// <summary>
        /// 不会在AssetBundle中包含类型信息;
        /// </summary>
        public bool DisableWriteTypeTree;
        /// <summary>
        /// 使用存储在Asset Bundle中的对象的id的哈希构建Asset Bundle;
        /// </summary>
        public bool DeterministicAssetBundle;
        /// <summary>
        /// 强制重建Asset Bundles;
        /// </summary>
        public bool ForceRebuildAssetBundle;
        /// <summary>
        /// 执行增量构建检查时忽略类型树更改;
        /// </summary>
        public bool IgnoreTypeTreeChanges;
        /// <summary>
        /// AB打包输出的绝对路径；
        /// </summary>
        public string AssetBundleBuildPath;
        /// <summary>
        /// AB输出目录；
        /// </summary>
        public string BuildPath;
        /// <summary>
        /// 打包的版本；
        /// </summary>
        public string BuildVersion;
        /// <summary>
        /// 内部构建的版本号
        /// </summary>
        public int InternalBuildVersion;
        /// <summary>
        /// AB偏移加密；
        /// </summary>
        public bool AssetBundleEncryption;
        /// <summary>
        /// AB偏移量；
        /// </summary>
        public int AssetBundleOffsetValue;
        /// <summary>
        /// 打包输出资源加密；
        /// </summary>
        public bool BuildedAssetsEncryption;
        /// <summary>
        /// 打包输出资源加密密钥；
        /// </summary>
        public string BuildIedAssetsEncryptionKey;
        /// <summary>
        /// 拷贝到streamingAsset文件；
        /// </summary>
        public bool CopyToStreamingAssets;
        /// <summary>
        /// 使用StreamingAsset相对路径；
        /// </summary>
        public bool UseStreamingAssetsRelativePath;
        /// <summary>
        /// 拷贝到的StreamingAssets相对路径；
        /// </summary>
        public string StreamingAssetsRelativePath;
        public AssetBundleTabData()
        {
            AssetBundleCompressType = AssetBundleCompressType.ChunkBasedCompression_LZ4;
            BuildTarget = BuildTarget.StandaloneWindows;
            BuildHandlerName = Constants.NONE;
            BuildHandlerIndex = 0;
            BuildPath = Utility.IO.WebPathCombine(EditorUtil.ApplicationPath(), "AssetBundles"); AssetBundleEncryption = false;
            AssetBundleOffsetValue = 16;
            BuildedAssetsEncryption = false;
            BuildIedAssetsEncryptionKey = "CosmosBundlesKey";
            AssetBundleNameType = AssetBundleNameType.HashInstead;
            BuildVersion = "0.0.1";
            InternalBuildVersion = 0;
            ForceRebuildAssetBundle = false;
            DisableWriteTypeTree = false;
            DeterministicAssetBundle = false;
            IgnoreTypeTreeChanges = false;
            StreamingAssetsRelativePath = BuildVersion;
            AssetBundleBuildPath = Utility.IO.WebPathCombine(BuildPath, $"{BuildVersion}_{InternalBuildVersion}");
        }
    }
}
