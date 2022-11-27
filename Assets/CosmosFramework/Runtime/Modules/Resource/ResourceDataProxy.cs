namespace Cosmos.Resource
{
    internal class ResourceDataProxy
    {
        public static ulong EncryptionOffset { get; set; }
        public static string BundlePath { get; set; }
        public static string BuildInfoEncryptionKey { get; set; }
        public  static ResourceBundlePathType ResourceBundlePathType { get; set; }
        public  static ResourceLoadMode  ResourceLoadMode{ get; set; }
    }
}
