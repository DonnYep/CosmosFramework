namespace Cosmos
{
    /// <summary>
    /// AB包中场景资源的信息；
    /// </summary>
    public sealed class SceneAssetInfo : AssetInfo
    {
        public SceneAssetInfo(string assetBundleName, string sceneName) : base(assetBundleName, sceneName, null){}
    }
}