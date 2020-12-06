namespace Cosmos
{
    /// <summary>
    /// 资源信息基类
    /// </summary>
    public class AssetInfo: AssetInfoBase
    {
        public AssetInfo(string assetBundleName, string assetPath, string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {
        }
    }
}