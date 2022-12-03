using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cosmos.Resource
{
    public class ResourceUtility
    {
        /// <summary>
        /// 包名过滤
        /// </summary>
        /// <param name="bundleName">原始包名</param>
        /// <returns>过滤后的名</returns>
        public static string FilterName(string bundleName)
        {
            return bundleName.Replace("\\", "_").Replace("/", "_").Replace(".", "_").Replace(",","_").Replace(";","_").ToLower();
        }
        /// <summary>
        /// 获取文件夹的MD5；
        /// </summary>
        /// <param name="srcPath">文件夹路径</param>
        /// <returns>MD5</returns>
        public static string CreateDirectoryMd5(string srcPath)
        {
            var filePaths = Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();
            using (var md5 = MD5.Create())
            {
                foreach (var filePath in filePaths)
                {
                    byte[] pathBytes = Encoding.UTF8.GetBytes(filePath);
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                    byte[] contentBytes = File.ReadAllBytes(filePath);
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
                md5.TransformFinalBlock(new byte[0], 0, 0);
                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
        /// <summary>
        /// 生成对称加密的密钥；
        /// </summary>
        /// <param name="srcKey"></param>
        /// <returns></returns>
        public static byte[] GenerateBytesAESKey(string srcKey)
        {
            var keyLength = Encoding.UTF8.GetBytes(srcKey).Length;
            byte[] key = new byte[0];
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
    }
}
