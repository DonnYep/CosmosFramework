namespace Cosmos
{
    /// <summary>
    /// Unity资源信息类
    /// </summary>
    public class AssetInfo: AssetInfoBase
    {
        public AssetInfo(string assetBundleName, string assetPath, string resourcePath) 
            : base(assetBundleName, assetPath, resourcePath){}
        public AssetInfo() : base() { }
    }
}