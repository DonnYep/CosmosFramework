using System;
using System.Text;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class Text
        {
            [ThreadStatic]//每个静态类型字段对于每一个线程都是唯一的
            static StringBuilder stringBuilderCache;
            public static StringBuilder StringBuilderCache
            {
                get
                {
                    if (stringBuilderCache == null)
                        stringBuilderCache = new StringBuilder(1024);
                    return stringBuilderCache;
                }
                set { stringBuilderCache = value; }
            }
            /// <summary>
            /// 格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            /// <returns>格式化后的字符串</returns>
            public static string Format(string format, params object[] args)
            {
                if (format == null)
                {
                    throw new ArgumentNullException("Format is invalid.");
                }
                if (args == null)
                {
                    throw new ArgumentNullException("Args is invalid.");
                }
                StringBuilderCache.Length = 0;
                StringBuilderCache.AppendFormat(format, args);
                return StringBuilderCache.ToString();
            }
            /// <summary>
            /// 格式化字符串
            /// </summary>
            /// <param name="format">需要格式化的字符串</param>
            /// <param name="arg">额外的参数</param>
            /// <returns></returns>
            public static string Format(string format, object arg)
            {
                if (string.IsNullOrEmpty(format))
                {
                    throw new ArgumentNullException("Format is invalid.");
                }
                if (arg == null)
                {
                    throw new ArgumentNullException("Arg is invalid.");
                }
                StringBuilderCache.Length = 0;
                StringBuilderCache.AppendFormat(format, arg);
                return StringBuilderCache.ToString();
            }
            public static string Append(params object[] args)
            {
                if (args == null)
                {
                    throw new ArgumentNullException("Append is invalid.");
                }
                StringBuilderCache.Length = 0;
                int length = args.Length;
                for (int i = 0; i < length; i++)
                {
                    StringBuilderCache.Append(args[i]);
                }
                return StringBuilderCache.ToString();
            }
            public static void ClearStringBuilder()
            {
                StringBuilderCache.Clear();
            }
            /// <summary>
            /// 是否是一串数字类型的string
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static bool IsNumeric(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return false;
                for (int i = 0; i < str.Length; i++)
                {
                    if (!char.IsNumber(str[i])) return false;
                }
                return true;
            }
            /// <summary>
            /// 分割字符串
            /// </summary>
            /// <param name="fullString">完整字段</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <param name="removeEmptyEntries">是否返回分割后数组中的空元素</param>
            /// <param name="subStringIndex">分割后数组的序号</param>
            /// <returns>分割后的字段</returns>
            public static string StringSplit(string fullString, String[] separator, bool removeEmptyEntries, int subStringIndex)
            {
                string[] stringArray = null;
                if (removeEmptyEntries)
                    stringArray = fullString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                else
                    stringArray = fullString.Split(separator, StringSplitOptions.None);
                string subString = stringArray[subStringIndex];
                return subString;
            }
            /// <summary>
            /// 分割字符串
            /// </summary>
            /// <param name="fullString">完整字段</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <param name="count">要返回的子字符串的最大数量</param>
            /// <param name="removeEmptyEntries">是否移除空实体</param>
            /// <returns>分割后的字段</returns>
            public static string StringSplit(string fullString, String[] separator, int count, bool removeEmptyEntries)
            {
                string[] stringArray = null;
                if (removeEmptyEntries)
                    stringArray = fullString.Split(separator, count, StringSplitOptions.RemoveEmptyEntries);
                else
                    stringArray = fullString.Split(separator, count, StringSplitOptions.None);
                return stringArray.ToString();
            }
            /// <summary>
            /// 分割字符串
            /// </summary>
            /// <param name="fullString">分割字符串</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <returns>分割后的字段数组</returns>
            public static string[] StringSplit(string fullString, String[] separator)
            {
                string[] stringArray = null;
                stringArray = fullString.Split(separator, StringSplitOptions.None);
                return stringArray;
            }
            public static int CharCount(string fullString, char separator)
            {
                if (string.IsNullOrEmpty(fullString) || string.IsNullOrEmpty(separator.ToString()))
                {
                    throw new ArgumentNullException("charCount \n string invaild!");
                }
                int count = 0;
                for (int i = 0; i < fullString.Length; i++)
                {
                    if (fullString[i] == separator)
                    {
                        count++;
                    }
                }
                return count;
            }
            /// <summary>
            /// 获取内容在UTF8编码下的字节长度；
            /// </summary>
            /// <param name="context">需要检测的内容</param>
            /// <returns>字节长度</returns>
            public static int GetUTF8Length(string context)
            {
                return Encoding.UTF8.GetBytes(context).Length;
            }
            /// <summary>
            /// 是否包含字符串验证
            /// </summary>
            /// <param name="context">传入的内容</param>
            /// <param name="values">需要检测的字符数组</param>
            /// <returns>是否包含</returns>
            public static bool StringContans(string context,string[] values)
            {
                var length = values.Length;
                for (int i = 0; i < length; i++)
                {
                    if (context.Contains(values[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            public static bool IsStringValid(string context)
            {
                if (string.IsNullOrEmpty(context))
                    return false;
                return true;
            }
            public static void IsStringValid(string context,string exceptionContext)
            {
                if (!string.IsNullOrEmpty(context))
                    throw new ArgumentNullException(exceptionContext);
            }
        }
    }
}
