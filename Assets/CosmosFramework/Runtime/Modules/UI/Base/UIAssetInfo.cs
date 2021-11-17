namespace Cosmos.UI
{
    /// <summary>
    /// UI资源信息；
    /// </summary>
    public class UIAssetInfo : AssetInfo
    {
        readonly string uiAssetName;
        public string UIAssetName { get { return uiAssetName; } }
        public string UIGroupName{ get; set; }
        public UIAssetInfo(string uiAssetName,string assetPath):base(assetPath)
        {
            this.uiAssetName= uiAssetName;
        }
        public UIAssetInfo(string uiAssetName, string assetBundleName, string assetPath) : base(assetBundleName, assetPath)
        {
            this.uiAssetName = uiAssetName;
        }
    }
}
