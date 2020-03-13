using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
namespace Cosmos
{
    public sealed partial class  Utility
    {
        //TODO  编码类扩充
        public sealed class Encode
        {
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
            public static byte[] ConvertToByte(float  value)
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
            static StringBuilder stringBuilderCache;
            static StringBuilder StringBuilderCache
            {
                get
                {
                    if (stringBuilderCache == null)
                        stringBuilderCache = new StringBuilder(1024);
                    return stringBuilderCache;
                }
                set { stringBuilderCache = value; }
            }
            public static string ConvertToBinary(int value)
            {
                StringBuilderCache.Clear();
                StringBuilderCache.Append(Convert.ToString(value, 2));
                return StringBuilderCache.ToString();
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