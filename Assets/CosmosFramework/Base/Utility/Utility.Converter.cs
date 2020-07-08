using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public  sealed partial class Utility
    {
        public static class Converter
        {
            //TODO Converter 转换工具需要完善
            public static string GetString(byte[] value)
            {
                if (value == null)
                {
                    throw new CFrameworkException("Value is invalid");
                }
                return Encoding.UTF8.GetString(value);
            }
            public static byte[] GetBytes(string value)
            {
                return Encoding.UTF8.GetBytes(value);
            }
            //public static byte[] GetBytes(bool value)
            //{

            //}
            public static void GetBytes(bool value,byte[] buffer,int startIndex)
            {
                if (buffer == null)
                {
                    throw new CFrameworkException("Buffer is invalid.");
                }
                if(startIndex < 0 || startIndex + 1 > buffer.Length)
                {
                    throw new CFrameworkException("Start index is invalid.");
                }
                buffer[startIndex] = value ? (byte)1 : (byte)0;
            }
            public static string GetString(byte[] value,int startIndex,int length)
            {
                if (value == null)
                {
                    throw new CFrameworkException("Value is invalid.");
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
        }
    }
}
