using System.Runtime.InteropServices;
using System;
namespace Cosmos.UI
{
    /// <summary>
    /// UI资源信息；
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct UIAssetInfo : IEquatable<UIAssetInfo>
    {
        public string AssetName { get; private set; }
        public string UIFormName { get; private set; }
        public string UIGroupName { get; private set; }
        public UIAssetInfo(string assetName)
    : this(assetName, assetName, string.Empty) { }
        public UIAssetInfo(string assetName, string uiGroupName)
            : this(assetName, assetName, uiGroupName) { }
        public UIAssetInfo(string assetName, string uiFormName, string uiGroupName)
        {
            AssetName = assetName;
            UIFormName = uiFormName;
            UIGroupName = uiGroupName;
        }
        public bool Equals(UIAssetInfo other)
        {
            return this.AssetName == other.AssetName &&
                this.UIFormName == other.AssetName &&
                this.UIGroupName == other.UIGroupName;
        }
    }
}
