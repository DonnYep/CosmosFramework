namespace Cosmos
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public sealed class ResourceAssetInfo : AssetInfo
    {
        public ResourceAssetInfo(string assetBundleName, string assetPath, string resourcePath) 
            : base(assetBundleName, assetPath, resourcePath)
        {
        }
    }
}
