namespace Cosmos
{
    /// <summary>
    /// 预制体资源信息
    /// </summary>
    public sealed class ResourceAssetInfo : AssetInfo
    {
        public ResourceAssetInfo(string resourcePath) : base(resourcePath){}
        public ResourceAssetInfo(string assetBundleName, string assetPath, string resourcePath) 
            : base(assetBundleName, assetPath, resourcePath){}
    }
}
