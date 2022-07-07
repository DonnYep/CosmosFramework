namespace Cosmos.Resource
{
    public class ResourceDataProxy : Singleton<ResourceDataProxy>
    {
        public ulong EncryptionOffset { get; set; }
        public string BundlePath { get; set; }
        public string BuildInfoEncryptionKey{ get; set; }
    }
}
