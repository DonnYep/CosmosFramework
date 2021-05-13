namespace Cosmos
{
    /// <summary>
    /// UI资源信息；
    /// </summary>
    public class UIAssetInfo : AssetInfo
    {
        public string UIAssetName { get; set; }
        public UIAssetInfo(string uiAssetName)
        {
            this.UIAssetName = uiAssetName;
        }
        public UIAssetInfo(string uiAssetName, string assetBundleName, string assetPath,  string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {
            this.UIAssetName = uiAssetName;
        }
    }
}
