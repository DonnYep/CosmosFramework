using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class Converter
        {
            static StringBuilder stringBuilderCache = new StringBuilder(1024);
            /// <summary>
            ///从UFT-8编码到Base64； 
            /// </summary>
            /// <param name="message">待编码的信息</param>
            /// <returns>编码后的信息</returns>
            public static string EnecodeBase64String(string message)
            {
                var msg = Encoding.GetEncoding("utf-8").GetBytes(message);
                return Convert.ToBase64String(msg);
            }
            /// <summary>
            /// 从Base64解码到UTF-8格式
            /// </summary>
            /// <param name="message">待解码的信息</param>
            /// <returns>解码后的信息</returns>
            public static string DecodeBase64String(string message)
            {
                byte[] bytes = Convert.FromBase64String(message);
                return Encoding.GetEncoding("utf-8").GetString(bytes);
            }
            public static string ConvertToHexString(byte[] bytes)
            {
                string hexString = string.Empty;
                if (bytes != null)
                {
                    foreach (byte b in bytes)
                    {
                        stringBuilderCache.AppendFormat("{0:x2}", b);
                    }
                    hexString = stringBuilderCache.ToString();
                }
                return hexString;
            }
            /// <summary>
            /// 约束数值长度，少增多减；
            /// 例如128约束5位等于12800，1024约束3位等于102；
            /// </summary>
            /// <param name="srcValue">原始数值</param>
            /// <param name="length">需要保留的长度</param>
            /// <returns>修改后的int数值</returns>
            public static long RetainInt64(long srcValue, ushort length)
            {
                if (length == 0)
                    return 0;
                var len = srcValue.ToString().Length;
                if (len > length)
                {
                    string sub = srcValue.ToString().Substring(0, length);
                    return long.Parse(sub);
                }
                else
                {
                    var result = srcValue * (long)Math.Pow(10, length - len);
                    return result;
                }
            }
            public static int RetainInt32(int srcValue, ushort length)
            {
                if (length == 0)
                    return 0;
                var len = srcValue.ToString().Length;
                if (len > length)
                {
                    string sub = srcValue.ToString().Substring(0, length);
                    return int.Parse(sub);
                }
                else
                {
                    var result = srcValue * (int)Math.Pow(10, length - len);
                    return result;
                }
            }
            /// <summary>
            /// 转换byte长度到对应单位；
            /// </summary>
            /// <param name="bytes">byte长度</param>
            /// <param name="decimals">保留的小数长度</param>
            /// <returns>格式化后的单位</returns>
            public static string FormatBytesSize(long bytes, int decimals = 2)
            {
                string[] suffix = { "Byte", "KB", "MB", "GB", "TB" };
                int i = 0;
                double dblSByte = bytes;
                if (bytes > 1024)
                    for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                        dblSByte = bytes / 1024.0;
                return $"{Math.Round(dblSByte, decimals)}{suffix[i]}";
            }
            /// <summary>
            /// object类型转换为bytes
            /// </summary>
            /// <param name="obj">对象</param>
            /// <returns>byte数组</returns>
            public static byte[] Object2Bytes(object obj)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    return ms.GetBuffer();
                }
            }
        }
    }
}
