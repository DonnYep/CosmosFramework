namespace Cosmos.Resource
{
    public class ResourceMergedBundleBuildInfo
    {
        /// <summary>
        /// Path of the file on disk
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
