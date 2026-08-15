using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源核心库工具类。
    /// <para>完全自包含：不依赖框架其他程序集，仅依赖 UnityEngine 与 System。</para>
    /// </summary>
    public static partial class ResourceUtility
    {
        public static string Prefix
        {
            get
            {
                string prefix = string.Empty;
#if UNITY_IOS || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                prefix = @"file://";
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
            if (string.IsNullOrEmpty(bundleName))
                return bundleName;
            return bundleName.Replace("\\", "_").Replace("/", "_").Replace(".", "_").Replace(",", "_").Replace(";", "_").ToLower();
        }
        /// <summary>
        /// 生成对称加密的密钥
        /// <para>16/24/32字节密钥直接使用；其余长度统一取MD5，保证编辑器与运行时一致。</para>
        /// </summary>
        /// <param name="srcKey">密钥文本</param>
        /// <returns>生成的密钥</returns>
        public static byte[] GenerateBytesAESKey(string srcKey)
        {
            if (string.IsNullOrEmpty(srcKey))
                return new byte[0];
            var keyBytes = Encoding.UTF8.GetBytes(srcKey);
            if (keyBytes.Length == 16 || keyBytes.Length == 24 || keyBytes.Length == 32)
                return keyBytes;
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(keyBytes);
            }
        }
        /// <summary>
        /// 检测文件清单的密钥key是否合法
        /// </summary>
        /// <param name="aesKey">密钥</param>
        /// <returns>是否合法</returns>
        public static bool CheckManifestKeyValidable(string aesKey)
        {
            if (string.IsNullOrEmpty(aesKey))
                return true;
            var keyBytes = Encoding.UTF8.GetBytes(aesKey);
            return keyBytes.Length == 16 || keyBytes.Length == 24 || keyBytes.Length == 32;
        }
        /// <summary>
        /// 获取Unity对象运行时内存大小
        /// </summary>
        public static long GetRuntimeMemorySizeLong(UnityEngine.Object @object)
        {
            if (@object == null)
                return 0;
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(@object);
        }

        #region AES
        /// <summary>
        /// AES字符串加密（CBC + PKCS7）
        /// </summary>
        public static string AESEncryptStringToString(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;
            if (key == null || key.Length == 0)
                return plainText;
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = GetIV(key);
                    var encryptor = aes.CreateEncryptor();
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"AESEncryptStringToString failed : {e}");
                return null;
            }
        }
        /// <summary>
        /// AES字符串解密（CBC + PKCS7）
        /// </summary>
        public static string AESDecryptStringToString(string cipherText, byte[] key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
            if (key == null || key.Length == 0)
                return cipherText;
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.IV = GetIV(key);
                    var decryptor = aes.CreateDecryptor();
                    var cipherBytes = Convert.FromBase64String(cipherText);
                    var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"AESDecryptStringToString failed : {e}");
                return null;
            }
        }
        static byte[] GetIV(byte[] key)
        {
            var iv = new byte[16];
            var copyLength = Math.Min(key.Length, 16);
            Array.Copy(key, iv, copyLength);
            return iv;
        }
        #endregion

        #region IO（自包含文件工具）
        /// <summary>
        /// 路径拼接
        /// </summary>
        public static string PathCombine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;
            var path = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                path = Path.Combine(path, paths[i]);
            }
            return path;
        }
        /// <summary>
        /// 创建文件夹（不存在时）
        /// </summary>
        public static void CreateFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        /// <summary>
        /// 删除文件夹（存在时）
        /// </summary>
        public static void DeleteFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        /// <summary>
        /// 读取文本文件内容（UTF-8无BOM）
        /// </summary>
        public static string ReadTextFileContent(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            if (!File.Exists(path))
                return null;
            return File.ReadAllText(path, new UTF8Encoding(false));
        }
        /// <summary>
        /// 覆写文本文件（UTF-8无BOM）
        /// </summary>
        public static void OverwriteTextFile(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
                return;
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                CreateFolder(directory);
            File.WriteAllText(path, content ?? string.Empty, new UTF8Encoding(false));
        }
        /// <summary>
        /// 计算文件MD5
        /// </summary>
        public static string GenerateFileMD5(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return string.Empty;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }
        /// <summary>
        /// 计算字符串MD5
        /// </summary>
        public static string GenerateStringMD5(string context)
        {
            if (string.IsNullOrEmpty(context))
                return string.Empty;
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(context);
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }
        #endregion

        #region PackageManifest
        /// <summary>
        /// 序列化文件清单
        /// </summary>
        /// <param name="manifest">清单对象</param>
        /// <param name="aesKey">加密密钥，可为空</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeManifest(PackageManifest manifest, string aesKey)
        {
            if (manifest == null)
                return null;
            var json = JsonUtility.ToJson(manifest);
            var keyBytes = GenerateBytesAESKey(aesKey);
            var hasKey = keyBytes != null && keyBytes.Length > 0;
            if (hasKey)
                return AESEncryptStringToString(json, keyBytes);
            return json;
        }

        /// <summary>
        /// 反序列化文件清单
        /// </summary>
        /// <param name="context">清单字符串</param>
        /// <param name="aesKey">加密密钥，可为空</param>
        /// <returns>反序列化后的清单对象</returns>
        public static PackageManifest DeserializeManifest(string context, string aesKey)
        {
            if (string.IsNullOrEmpty(context))
                return null;
            PackageManifest manifest = null;
            try
            {
                var keyBytes = GenerateBytesAESKey(aesKey);
                var hasKey = keyBytes != null && keyBytes.Length > 0;
                var srcContext = context;
                if (hasKey)
                    srcContext = AESDecryptStringToString(context, keyBytes);
                if (string.IsNullOrEmpty(srcContext))
                    return null;
                manifest = JsonUtility.FromJson<PackageManifest>(srcContext);
            }
            catch (Exception e)
            {
                Debug.LogError($"DeserializeManifest failed : {e}");
            }
            return manifest;
        }
        #endregion

        #region Path
        /// <summary>
        /// 加载文件清单内容。
        /// <para>支持绝对路径与相对StreamingAssets路径；http(s)远端清单暂不支持（可配合WebRequest模块自行下载后传入）。</para>
        /// </summary>
        public static string LoadManifestContext(string manifestPath)
        {
            if (string.IsNullOrEmpty(manifestPath))
                throw new ArgumentNullException("ManifestPath is invalid !");
            if (manifestPath.StartsWith("http://") || manifestPath.StartsWith("https://"))
                throw new NotSupportedException($"Remote manifest loading is not supported yet : {manifestPath}");
            var path = ResolveRuntimePath(manifestPath);
            return ReadTextFileContent(path);
        }

        /// <summary>
        /// 获取AssetBundle文件路径
        /// </summary>
        public static string GetBundleFilePath(ResourceInitParameters initParameters, string bundleName)
        {
            if (initParameters == null || string.IsNullOrEmpty(bundleName))
                return null;
            var directory = initParameters.BundleDirectory;
            if (string.IsNullOrEmpty(directory))
                directory = CFResourceConstants.DEFAULT_STREAMING_ASSETS_RELATIVE_PATH;
            var dirPath = ResolveRuntimePath(directory);
            var fileName = $"{FilterName(bundleName)}.{CFResourceConstants.DEFAULT_AB_EXTENSION}";
            return PathCombine(dirPath, fileName);
        }

        /// <summary>
        /// 解析运行时路径：
        /// <para>绝对路径原样返回；相对路径优先persistentDataPath，其次streamingAssetsPath。</para>
        /// </summary>
        public static string ResolveRuntimePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            if (path.StartsWith("http://") || path.StartsWith("https://") || path.StartsWith("file://"))
                return path;
            if (Path.IsPathRooted(path))
                return path;
            var persistentPath = Path.Combine(Application.persistentDataPath, path);
            if (File.Exists(persistentPath))
                return persistentPath;
            return Path.Combine(Application.streamingAssetsPath, path);
        }
        #endregion
    }
}
