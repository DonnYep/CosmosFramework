namespace Cosmos.Resource
{
    public enum CFResourcePlayMode : byte
    {
        None,
        /// <summary>
        /// 编辑器模式
        /// </summary>
        AssetDatabase,
        /// <summary>
        /// 编辑器下的ab模式
        /// </summary>
        EditorAssetBundle,
        /// <summary>
        /// 打包发布ab模式
        /// </summary>
        AssetBundle,
        /// <summary>
        /// 离线模式，使用本地资产
        /// </summary>
        Offline
    }
}
