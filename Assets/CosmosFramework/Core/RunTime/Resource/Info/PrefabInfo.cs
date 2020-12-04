namespace Cosmos
{
    /// <summary>
    /// 预制体信息
    /// </summary>
    public sealed class PrefabInfo : AssetInfo
    {
        public PrefabInfo(string assetBundleName, string assetPath, string resourcePath) : base(assetBundleName, assetPath, resourcePath)
        {
        }
        public PrefabInfo(PrefabAssetAttribute att) : base(att.AssetBundleName, att.AssetPath, att.ResourcePath)
        {
        }
        public PrefabInfo(EntityAssetAttribute att) : base(att.AssetBundleName, att.AssetPath, att.ResourcePath)
        {
        }
    }
}
