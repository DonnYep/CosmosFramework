namespace Cosmos.Resource
{
    public class ResourceMergedBundleBuildInfo
    {
        /// <summary>
        /// Local path
        /// </summary>
        public string Path;
        /// <summary>
        /// AssetBundle offset
        /// </summary>
        public ulong Offset;
        /// <summary>
        ///  Build info of resource;
        /// </summary>
        public ResourceBundleBuildInfo ResourceBundleBuildInfo;
    }
}
