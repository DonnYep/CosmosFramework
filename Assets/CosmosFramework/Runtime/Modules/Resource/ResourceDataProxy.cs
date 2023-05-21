namespace Cosmos.Resource
{
    internal class ResourceDataProxy
    {
        public static ulong BundleEncryptionOffset { get; set; }
        public static string BundlePath { get; set; }
        public static string ManifestEncryptionKey { get; set; }
        public  static ResourceBundlePathType ResourceBundlePathType { get; set; }
        public  static ResourceLoadMode  ResourceLoadMode{ get; set; }
        public  static bool UnloadAllLoadedObjectsWhenBundleUnload { get; set; }
    }
}
