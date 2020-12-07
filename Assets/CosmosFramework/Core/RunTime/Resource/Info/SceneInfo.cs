namespace Cosmos
{
    /// <summary>
    /// 场景信息
    /// </summary>
    public sealed class SceneInfo : AssetInfo
    {
        public SceneInfo(string assetBundleName, string sceneName) : base(assetBundleName, sceneName, null){}
    }
}