namespace Cosmos.Resource
{
    /// <summary>
    /// Unity资源信息类
    /// </summary>
    public class AssetInfo : AssetInfoBase
    {
        public AssetInfo() { }
        public AssetInfo(string assetBundleName, string assetPath)
            : base(assetBundleName, assetPath) { }
        public AssetInfo(string assetPath) : base(string.Empty, assetPath) { }
    }
}