namespace Cosmos
{
    /// <summary>
    /// UI资源信息；
    /// </summary>
    public class UIAssetInfo : AssetInfo
    {
        readonly string uiAssetName;
        readonly string uiGroupName;
        public string UIAssetName { get { return uiAssetName; } }
        public string UIGroupName{ get { return uiGroupName; } }
        public UIAssetInfo(string uiAssetName)
        {
            this.uiAssetName= uiAssetName;
        }
        public UIAssetInfo(string uiAssetName,string uiGroupName)
        {
            this.uiAssetName = uiAssetName;
            this.uiGroupName= uiGroupName;
        }
        public UIAssetInfo(string uiAssetName, string uiGroupName, string assetBundleName, string assetPath,  string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {
            this.uiAssetName = uiAssetName;
            this.uiGroupName = uiGroupName;
        }
    }
}
