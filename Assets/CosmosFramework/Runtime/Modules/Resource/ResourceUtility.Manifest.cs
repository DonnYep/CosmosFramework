namespace Cosmos.Resource
{
    public static partial class ResourceUtility
    {
        public static class Manifest
        {
            public static string Serialize(ResourceManifest manifest, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return Serialize(manifest, keyBytes);
            }
            public static string Serialize(ResourceManifest manifest, byte[] keyBytes)
            {
                var manifestJson = LitJson.JsonMapper.ToJson(manifest);
                var hasKey = keyBytes != null && keyBytes.Length > 0;
                string context = string.Empty;
                if (hasKey)
                    context = Utility.Encryption.AESEncryptStringToString(manifestJson, keyBytes);
                else
                    context = manifestJson;
                return context;
            }
            public static ResourceManifest Deserialize(string manifestContext, string key)
            {
                var keyBytes = GenerateBytesAESKey(key);
                return Deserialize(manifestContext, keyBytes);
            }
            public static ResourceManifest Deserialize(string manifestContext, byte[] keyBytes)
            {
                ResourceManifest manifest = null;
                try
                {
                    var hasKey = keyBytes != null && keyBytes.Length > 0;
                    var context = manifestContext;
                    if (hasKey)
                    {
                        context = Utility.Encryption.AESDecryptStringToString(manifestContext, keyBytes);
                    }
                    manifest = LitJson.JsonMapper.ToObject<ResourceManifest>(context);
                }
                catch { }
                return manifest;
            }
        }
    }
}
