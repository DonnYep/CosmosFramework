using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Quark
{
    public partial class QuarkUtility
    {
        /// <summary>
        /// 标准的UTF-8是不含BOM的；
        /// 构造的UTF8Encoding，排除掉UTF8-BOM的影响；
        /// </summary>
        static UTF8Encoding utf8Encoding = new UTF8Encoding(false);
        [ThreadStatic]//每个静态类型字段对于每一个线程都是唯一的
        static StringBuilder stringBuilderCache = new StringBuilder(1024);
        public static string Append(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("Append is invalid.");
            }
            stringBuilderCache.Clear();
            int length = args.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilderCache.Append(args[i]);
            }
            return stringBuilderCache.ToString();
        }
        public static string WebPathCombine(params string[] paths)
        {
            var pathResult = Path.Combine(paths);
            pathResult = pathResult.Replace("\\", "/");
            return pathResult;
        }
        /// <summary>
        /// 读取指定路径下某text类型文件的内容
        /// </summary>
        /// <param name="fileFullPath">文件的完整路径，包含文件名与扩展名</param>
        /// <returns>指定文件的包含的内容</returns>
        public static string ReadTextFileContent(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
                throw new IOException("ReadTextFileContent path not exist !" + fileFullPath);
            string result = string.Empty;
            using (FileStream stream = File.Open(fileFullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream, utf8Encoding))
                {
                    result = Append(reader.ReadToEnd());
                }
            }
            return result;
        }
        /// <summary>
        /// 获取文件大小；
        /// 若文件存在，则返回正确的大小；若不存在，则返回-1；
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns>文件long类型的长度</returns>
        public static long GetFileSize(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                return -1;
            }
            else if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return -1;
        }
        /// <summary>
        /// 完全覆写；
        ///  使用UTF8编码；
        /// </summary>
        /// <param name="fileFullPath">文件完整路径</param>
        /// <param name="context">写入的信息</param>
        public static void OverwriteTextFile(string fileFullPath, string context)
        {
            using (FileStream stream = File.Open(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                using (StreamWriter writer = new StreamWriter(stream, utf8Encoding))
                {
                    writer.WriteLine(context);
                    writer.Flush();
                }
            }
        }
        /// <summary>
        /// Ping URL是否存在；
        /// Ping的过程本身是阻塞的，谨慎使用！；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>是否存在</returns>
        public static bool PingURI(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="fullString">分割字符串</param>
        /// <param name="separator">new string[]{"."}</param>
        /// <returns>分割后的字段数组</returns>
        public static string[] StringSplit(string fullString, string[] separator)
        {
            string[] stringArray = null;
            stringArray = fullString.Split(separator, StringSplitOptions.None);
            return stringArray;
        }
        /// <summary>
        /// 字段合并；
        /// </summary>
        /// <param name="strings">字段数组</param>
        /// <returns></returns>
        public static string Combine(params string[] strings)
        {
            if (strings == null)
                throw new ArgumentNullException("Combine is invalid.");
            stringBuilderCache.Length = 0;
            int length = strings.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilderCache.Append(strings[i]);
            }
            return stringBuilderCache.ToString();
        }
        public static void IsStringValid(string context, string exceptionContext)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException(exceptionContext);
        }
        public static void DeleteFile(string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }
        }

        /// <summary>
        /// 格式化AB名称；
        /// 此方法Quark专供；
        /// </summary>
        /// <param name="srcStr">原始名称</param>
        /// <param name="replaceContext">替换的内容</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatAssetBundleName(string srcStr,string replaceContext="_")
        {
            return Replace(srcStr, new string[] { "\\", "/", ".", " " }, replaceContext).ToLower();
        }
        /// <summary>
        /// 多字符替换；
        /// </summary>
        /// <param name="context">需要修改的内容</param>
        /// <param name="oldContext">需要修改的内容</param>
        /// <param name="newContext">修改的新内容</param>
        /// <returns>修改后的内容</returns>
        public static string Replace(string context, string[] oldContext, string newContext)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid.");
            if (oldContext == null)
                throw new ArgumentNullException("oldContext is invalid.");
            if (string.IsNullOrEmpty(newContext))
                throw new ArgumentNullException("newContext is invalid.");
            var length = oldContext.Length;
            for (int i = 0; i < length; i++)
            {
                context = context.Replace(oldContext[i], newContext);
            }
            return context;
        }
        public static byte[] GenerateBytesAESKey(string srckey)
        {
            var srcKeyLen= Encoding.UTF8.GetBytes(srckey).Length;
            int dstLen = 16;
            switch (srcKeyLen)
            {
                case 0:
                    return new byte[0];
                    break;
                case 16:
                    dstLen = 16;
                    break;
                case 24:
                    dstLen = 24;
                    break;
                case 32:
                    dstLen = 32;
                    break;
                default:
                    throw new Exception("Key should be 16,24 or 32 bytes long");
                    break;
            }
            var srcBytes = Encoding.UTF8.GetBytes(srckey);
            byte[] dstBytes = new byte[dstLen];
            var srcLen = srcBytes.Length;
            if (srcLen > dstLen)
            {
                Array.Copy(srcBytes, 0, dstBytes, 0, dstLen);
            }
            else
            {
                var diffLen = dstLen - srcLen;
                var diffBytes = new byte[diffLen];
                Array.Copy(srcBytes, 0, dstBytes, 0, srcLen);
                Array.Copy(diffBytes, 0, dstBytes, srcLen, diffLen);
            }
            return dstBytes;
        }
        /// <summary>
        /// AES对称加密string类型内容;
        /// 密钥的byte长度必须是16, 24, 32；
        /// </summary>
        /// <param name="context">需要加密的内容</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的内容</returns>
        public static string AESEncryptStringToString(string context, byte[] key)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid ! ");
            if (key == null)
                throw new ArgumentNullException("key is invalid ! ");
            using (var aes = new AesCryptoServiceProvider())
            {
                var iv = aes.IV;
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var cryptStream = new CryptoStream(ms, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptStream))
                        {
                            writer.Write(context);
                        }
                    }
                    var buf = ms.ToArray();
                    return Convert.ToBase64String(buf, 0, buf.Length);
                }
            }
        }
        /// <summary>
        /// AES对称解密string类型内容；
        /// 密钥的byte长度必须是16, 24, 32；
        /// </summary>
        /// <param name="context">需要解密的内容</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的内容</returns>
        public static string AESDecryptStringToString(string context, byte[] key)
        {
            if (string.IsNullOrEmpty(context))
                throw new ArgumentNullException("context is invalid ! ");
            if (key == null)
                throw new ArgumentNullException("key is invalid ! ");
            var bytes = Convert.FromBase64String(context);
            using (var aes = new AesCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    var iv = new byte[16];
                    ms.Read(iv, 0, 16);
                    using (var cryptStream = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        #region Debug
        public static void LogInfo(object msg)
        {
            UnityEngine.Debug.Log($"<b><color=cyan>{"[QUARK-INFO]-->>"} </color></b>{msg}");
        }
        public static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning($"<b><color=orange>{"[QUARK-WARNING]-->>" }</color></b>{msg}");
        }
        public static void LogError(object msg)
        {
            UnityEngine.Debug.LogError($"<b><color=red>{"[QUARK-ERROR]-->>"} </color></b>{msg}");
        }
        #endregion
        #region Json
        /// <summary>
        /// 将对象序列化为JSON字段
        /// </summary>
        /// <param name="obj">需要被序列化的对象</param>
        /// <returns>序列化后的JSON字符串</returns>xxxx
        public static string ToJson(object obj, bool prettyPrint = false)
        {
            return LitJson.JsonMapper.ToJson(obj);
        }
        /// <summary>
        /// 将对象序列化为JSON流
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <returns>序列化后的JSON流</returns>
        public static byte[] ToJsonData(object obj)
        {
            return Encoding.UTF8.GetBytes(ToJson(obj));
        }
        /// <summary>
        /// 将JSON反序列化为泛型对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <returns>反序列化后的泛型对象</returns>
        public static T ToObject<T>(string json)
        {
            return LitJson.JsonMapper.ToObject<T>(json);
        }
        /// <summary>
        /// 将JSON字符串反序列化对象
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="json">需要反序列化的JSON字符串</param>
        /// <returns>反序列化后的对象</returns>
        public static object ToObject(string json, Type objectType)
        {
            return LitJson.JsonMapper.ToObject(json, objectType);
        }
        /// <summary>
        /// 将JSON流转换为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="jsonData">JSON流</param>
        /// <returns>反序列化后的对象</returns>
        public static T ToObject<T>(byte[] jsonData)
        {
            return ToObject<T>(Encoding.UTF8.GetString(jsonData));
        }
        #endregion
    }
}