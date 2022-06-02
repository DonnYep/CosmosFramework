using System.Runtime.InteropServices;
namespace Cosmos.UI
{
    /// <summary>
    /// UI资源信息；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct UIAssetInfo
    {
        public string AssetName { get; private set; }
        public string UIFormName { get; private set; }
        public string UIGroupName { get; private set; }
        public UIAssetInfo(string assetName, string uiGroupName)
            : this(assetName, assetName, uiGroupName) { }
        public UIAssetInfo(string assetName, string uiFormName, string uiGroupName)
        {
            AssetName = assetName;
            UIFormName = uiFormName;
            UIGroupName = uiGroupName;
        }
    }
}
