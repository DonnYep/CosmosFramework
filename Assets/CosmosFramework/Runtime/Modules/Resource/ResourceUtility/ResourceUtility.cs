using System.Text;

namespace Cosmos.Resource
{
    public static partial class ResourceUtility
    {
        public static string Prefix
        {
            get
            {
                string prefix = string.Empty;
#if UNITY_IOS||UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                prefix=@"file://";
#endif
                return prefix;
            }
        }
        /// <summary>
        /// 包名过滤
        /// </summary>
        /// <param name="bundleName">原始包名</param>
        /// <returns>过滤后的名</returns>
        public static string FilterName(string bundleName)
        {
            return bundleName.Replace("\\", "_").Replace("/", "_").Replace(".", "_").Replace(",", "_").Replace(";", "_").ToLower();
        }
        /// <summary>
        /// 生成对称加密的密钥
        /// </summary>
        /// <param name="srcKey"></param>
        /// <returns>生成的密钥</returns>
        public static byte[] GenerateBytesAESKey(string srcKey)
        {
            byte[] key = new byte[0];
            if (string.IsNullOrEmpty(srcKey))
                return key;
            var keyLength = Encoding.UTF8.GetBytes(srcKey).Length;
            if (keyLength == 8)
            {
                key = Utility.Encryption.Generate8BytesAESKey(srcKey);
            }
            else if (keyLength == 16)
            {
                key = Utility.Encryption.Generate16BytesAESKey(srcKey);
            }
            else if (keyLength == 24)
            {
                key = Utility.Encryption.Generate24BytesAESKey(srcKey);
            }
            else if (keyLength == 32)
            {
                key = Utility.Encryption.Generate32BytesAESKey(srcKey);
            }
            return key;
        }
        /// <summary>
        /// 检测文件清单的密钥key是否合法
        /// </summary>
        /// <param name="aesKey">密钥</param>
        /// <returns>是否合法</returns>
        public static bool CheckManifestKeyValidable(string aesKey)
        {
            var aesKeyLength = Encoding.UTF8.GetBytes(aesKey).Length;
            if (aesKeyLength != 0 && aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32)
                return false;
            else
                return true;
        }
    }
}
