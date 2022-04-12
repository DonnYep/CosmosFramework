using Cosmos;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
            return Utility.Text.Replace(srcStr, new string[] { "\\", "/", ".", " " }, replaceContext).ToLower();
        }

        #region Debug
        public static void LogInfo(object msg)
        {
            Utility.Debug.LogInfo(msg);
        }
        public static void LogWarning(object msg)
        {
            Utility.Debug.LogWarning(msg);
        }
        public static void LogError(object msg)
        {
            Utility.Debug.LogError(msg);
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