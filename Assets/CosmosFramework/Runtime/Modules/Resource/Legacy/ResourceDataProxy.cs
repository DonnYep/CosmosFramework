namespace Cosmos.Resource
{
    internal class ResourceDataProxy
    {
        public static ResourceBundlePathType ResourceBundlePathType { get; set; }
        public static ResourceLoadMode ResourceLoadMode { get; set; }
        public static bool UnloadAllLoadedObjectsWhenBundleUnload { get; set; }
        public static bool PrintLogWhenAssetNotExists { get; set; }
    }
}
