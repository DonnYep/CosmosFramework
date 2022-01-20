﻿namespace Cosmos
{
    public abstract class AssetInfoBase
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName { get; protected set; }
        /// <summary>
        /// Asset的路径
        /// </summary>
        public string AssetPath { get; protected set; }
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath { get; protected set; }
        public AssetInfoBase(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
        public AssetInfoBase(string resourcePath):this(string.Empty,string.Empty,resourcePath)
        {
            ResourcePath = resourcePath;
        }
        public void SetAssetInfo(AssetInfoBase assetInfo)
        {
            this.AssetBundleName = assetInfo.AssetBundleName;
            this.AssetPath = assetInfo.AssetPath;
            this.ResourcePath = assetInfo.ResourcePath;
        }
    }
}
