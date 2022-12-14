namespace Cosmos.Resource.Verifiy
{
    /// <summary>
    /// 文件清单校验产生的数据；
    /// </summary>
    public class ResourceManifestVerifiyInfo
    {
        /// <summary>
        /// 所以需要更新的大小；
        /// </summary>
        public long TotalSize;
        /// <summary>
        /// 需要更新包体的文件信息；
        /// </summary>
        public ResourceBundleVerifiyInfo[] ResourceVerifiyInfos;
    }
}
