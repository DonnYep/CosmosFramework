namespace Cosmos
{
    /// <summary>
    /// 资源信息基类
    /// </summary>
    public abstract class AssetInfo
    {
        /// <summary>
        /// AssetBundle的名称
        /// </summary>
        public string AssetBundleName;
        /// <summary>
        /// Asset的路径
        /// </summary>
        public string AssetPath;
        /// <summary>
        /// Resources文件夹中的路径
        /// </summary>
        public string ResourcePath;
        public AssetInfo(string assetBundleName, string assetPath, string resourcePath)
        {
            AssetBundleName = string.IsNullOrEmpty(assetBundleName) ? assetBundleName : assetBundleName.ToLower();
            AssetPath = assetPath;
            ResourcePath = resourcePath;
        }
    }
}