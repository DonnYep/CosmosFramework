﻿namespace Cosmos.Resource
{
    public abstract class AssetInfoBase
    {
        readonly string assetPath;
        readonly string assetBundleName;
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName { get { return assetBundleName; } }
        /// <summary>
        /// 资源的相对路径
        /// </summary>
        public string AssetPath { get { return assetPath; } }
        public AssetInfoBase() { }
        public AssetInfoBase(string assetPath) : this(string.Empty, assetPath) { }
        public AssetInfoBase(string assetBundleName, string assetPath)
        {
            this.assetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            this.assetPath = assetPath;
        }
    }
}
