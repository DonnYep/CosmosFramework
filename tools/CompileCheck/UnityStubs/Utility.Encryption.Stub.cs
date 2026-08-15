// Stub partial for Cosmos.Utility.Encryption used by the compile-check.
// The real implementation uses System.Runtime.Remoting which is unavailable in netstandard2.1.
using System.Security.Cryptography;
using System.Text;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Encryption
        {
            public static string GUID()
            {
                return System.Guid.NewGuid().ToString("N");
            }
            public static byte[] Generate8BytesAESKey(string key) { return new byte[8]; }
            public static byte[] Generate16BytesAESKey(string key) { return new byte[16]; }
            public static byte[] Generate24BytesAESKey(string key) { return new byte[24]; }
            public static byte[] Generate32BytesAESKey(string key) { return new byte[32]; }
            public static string GenerateMD5(byte[] bytes) { return string.Empty; }
            public static string GenerateMD5(string context) { return string.Empty; }
            public static string AESEncryptStringToString(string context, byte[] key) { return context; }
            public static string AESDecryptStringToString(string context, byte[] key) { return context; }
        }
    }
}
