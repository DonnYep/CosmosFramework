namespace Cosmos.Resource
{
    internal class ResourceDataProxy : Singleton<ResourceDataProxy>
    {
        public ulong EncryptionOffset { get; set; }
        public string BundlePath { get; set; }
        public string BuildInfoEncryptionKey { get; set; }
    }
}
