using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class Converter
        {
            [ThreadStatic]//每个静态类型字段对于每一个线程都是唯一的
            static StringBuilder stringBuilderCache = new StringBuilder(1024);
            /// <summary>
            /// 解码base64。
            /// </summary>
            /// <param name="context">需要解码的内容</param>
            /// <returns>解码后的内容</returns>
            public static string DecodeFromBase64(string context)
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(context));
            }
            /// <summary>
            /// 编码base64。
            /// </summary>
            /// <param name="context">需要编码的内容</param>
            /// <returns>编码后的内容</returns>
            public static string EncodeToBase64(string context)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(context));
            }
            /// <summary>
            /// 转换为16进制字符
            /// </summary>
            /// <param name="context">文本内容</param>
            /// <returns>16进制字符串</returns>
            public static string ConvertToHexString(string context)
            {
                string hexString = string.Empty;
                var bytes = Encoding.UTF8.GetBytes(context);
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
            /// 转换为16进制字符
            /// </summary>
            /// <param name="bytes">二进制内容</param>
            /// <returns>16进制字符串</returns>
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
            /// 约束数值长度，少增多减
            /// <para>例如128约束5位等于12800，1024约束3位等于102</para>
            /// </summary>
            /// <param name="srcValue">原始数值</param>
            /// <param name="length">需要保留的长度</param>
            /// <returns>修改后的long数值</returns>
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
            /// <summary>
            /// 约束数值长度，少增多减
            /// <para>例如128约束5位等于12800，1024约束3位等于102</para>
            /// </summary>
            /// <param name="srcValue">原始数值</param>
            /// <param name="length">需要保留的长度</param>
            /// <returns>修改后的int数值</returns>
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
            /// 转换byte长度到对应单位
            /// </summary>
            /// <param name="bytes">byte长度</param>
            /// <param name="decimalPlaces">保留的小数长度</param>
            /// <returns>格式化后的单位</returns>
            public static string FormatBytes(double bytes, int decimalPlaces = 2)
            {
                string[] sizeUnits = { "Byte", "KB", "MB", "GB", "TB", "PB", "EB" };
                double size = bytes;
                int unitIndex = 0;
                while (size >= 1024 && unitIndex < sizeUnits.Length - 1)
                {
                    size /= 1024;
                    unitIndex++;
                }
                string formatString = "0." + new string('0', decimalPlaces);

                return $"{Math.Round(size, decimalPlaces).ToString(formatString)}{sizeUnits[unitIndex]}";
            }
            /// <summary>
            /// object类型转换为bytes。
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
            /// <summary>
            /// byte转换为string字符
            /// </summary>
            /// <param name="bytes">byte内容</param>
            /// <returns>转换后的字符</returns>
            public static string ConvertToString(byte[] bytes)
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
