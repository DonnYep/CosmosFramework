using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
namespace Cosmos
{
    public sealed partial class Utility
    {
        //TODO  编码类扩充
        public static class Encode
        {
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
            //TODO添加其他进制的封装函数
            //public static string ConvertToOctal(int value)
            //{
            //    string result = Convert.ToString(value, 2);
            //    return result;
            //}
            //public static string ConvertToDecimal(int value)
            //{
            //    string result = Convert.ToString(value, 2);
            //    return result;
            //}
            //public static string ConvertToHexadecimal(int value)
            //{
            //    string result = Convert.ToString(value, 2);
            //    return result;
            //}
        }
    }
}