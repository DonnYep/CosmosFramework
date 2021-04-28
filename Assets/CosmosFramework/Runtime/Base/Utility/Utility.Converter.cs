using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class Converter
        {
            //TODO Converter 转换工具需要完善
            public static string GetString(byte[] value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value is invalid");
                }
                return Encoding.UTF8.GetString(value);
            }
            public static byte[] GetBytes(string value)
            {
                return Encoding.UTF8.GetBytes(value);
            }
            public static void GetBytes(bool value, byte[] buffer, int startIndex)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("Buffer is invalid.");
                }
                if (startIndex < 0 || startIndex + 1 > buffer.Length)
                {
                    throw new ArgumentNullException("Start index is invalid.");
                }
                buffer[startIndex] = value ? (byte)1 : (byte)0;
            }
            public static string GetString(byte[] value, int startIndex, int length)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value is invalid.");
                }
                return Encoding.UTF8.GetString(value, startIndex, length);
            }
            /// <summary>
            ///T是一个类的对象，由object转换成class对象 
            /// </summary>
            public static T ConvertToObject<T>(object arg)
                where T : class
            {
                return arg as T;
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static int Int(object arg)
            {
                return Convert.ToInt32(arg);
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static float Float(object arg)
            {
                return (float)System.Math.Round(Convert.ToSingle(arg));
            }
            /// <summary>
            /// object类型转换
            /// </summary>
            public static long Long(object arg)
            {
                return Convert.ToInt64(arg);
            }
            /// <summary>
            /// 解码到标准UTF-8格式
            /// </summary>
            public static string DecodeString(string message)
            {
                byte[] bytes = Convert.FromBase64String(message);
                return Encoding.GetEncoding("utf-8").GetString(bytes);
            }
            /// <summary>
            /// 编码到标准UTF-8格式
            /// </summary>
            public static string EncodeString(string message)
            {
                byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
                return Convert.ToBase64String(bytes);
            }
            public static byte[] ConvertToByte(string value)
            {
                byte[] byteArray = Encoding.Default.GetBytes(value);
                return byteArray;
            }
            public static byte[] ConvertToByte(int value)
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return byteArray;
            }
            public static byte[] ConvertToByte(float value)
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return byteArray;
            }
            public static byte[] ConvertToByte(short value)
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return byteArray;
            }
            public static byte[] ConvertToByte(ushort value)
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return byteArray;
            }
            public static byte[] ConvertToByte(bool value)
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                return byteArray;
            }
            public static string ConvertToString(int value)
            {
                return Utility.Text.Format(Convert.ToString(value, 2));
            }
            public static string ConvertToString(byte[] value)
            {
                string str = BitConverter.ToString(value);
                return str;
            }
            /// <summary>
            /// 转换成16进制的字符串
            /// </summary>
            /// <param name="bytes">byte数据</param>
            /// <returns>转换后的16进制string</returns>
            public static string ConvertToHexString(byte[] bytes)
            {
                string hexString = string.Empty;
                if (bytes != null)
                {
                    Utility.Text.ClearStringBuilder();
                    foreach (byte b in bytes)
                    {
                        Utility.Text.StringBuilderCache.AppendFormat("{0:x2}", b);
                    }
                    hexString = Utility.Text.StringBuilderCache.ToString();
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
                    var result= srcValue * (long)Math.Pow(10,length - len);
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
        }
    }
}
