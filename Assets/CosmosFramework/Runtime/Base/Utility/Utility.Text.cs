﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class Text
        {
            /// <summary>
            /// 每个静态类型字段对于每一个线程都是唯一的
            /// </summary>
            [ThreadStatic]
            static StringBuilder stringBuilderCache = new StringBuilder(1024);
            static char[] stringConstant ={
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
            };
            /// <summary>
            /// 生成指定长度的随机字符串
            /// </summary>
            /// <param name="length">字符串长度</param>
            /// <returns>生成的随机字符串</returns>
            public static string GenerateRandomString(int length)
            {
                stringBuilderCache.Clear();
                Random rd = new Random();
                for (int i = 0; i < length; i++)
                {
                    stringBuilderCache.Append(stringConstant[rd.Next(62)]);
                }
                return stringBuilderCache.ToString();
            }
            /// <summary>
            /// 格式化合并
            /// </summary>
            /// <param name="args">格式化内容</param>
            /// <returns>合并后的文本</returns>
            public static string FormatCombine(params object[] args)
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
            /// <summary>
            /// 字段合并
            /// </summary>
            /// <param name="strings">字段数组</param>
            /// <returns>合并后的文本</returns>
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
            /// <summary>
            /// 字段合并
            /// </summary>
            /// <param name="strings">字段数组</param>
            /// <returns>合并后的文本</returns>
            public static string Combine(IEnumerable<string> strings)
            {
                if (strings == null)
                    throw new ArgumentNullException("Combine is invalid.");
                stringBuilderCache.Clear();
                foreach (var str in strings)
                {
                    stringBuilderCache.Append(str);
                }
                return stringBuilderCache.ToString();
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
            /// <param name="context">完整字段</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <param name="removeEmptyEntries">是否返回分割后数组中的空元素</param>
            /// <param name="subStringIndex">分割后数组的序号</param>
            /// <returns>分割后的字段</returns>
            public static string StringSplit(string context, string[] separator, bool removeEmptyEntries, int subStringIndex)
            {
                string[] stringArray = null;
                if (removeEmptyEntries)
                    stringArray = context.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                else
                    stringArray = context.Split(separator, StringSplitOptions.None);
                string subString = stringArray[subStringIndex];
                return subString;
            }
            /// <summary>
            /// 分割字符串
            /// </summary>
            /// <param name="context">完整字段</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <param name="count">要返回的子字符串的最大数量</param>
            /// <param name="removeEmptyEntries">是否移除空实体</param>
            /// <returns>分割后的字段</returns>
            public static string StringSplit(string context, string[] separator, int count, bool removeEmptyEntries)
            {
                string[] stringArray = null;
                if (removeEmptyEntries)
                    stringArray = context.Split(separator, count, StringSplitOptions.RemoveEmptyEntries);
                else
                    stringArray = context.Split(separator, count, StringSplitOptions.None);
                return stringArray.ToString();
            }
            /// <summary>
            /// 分割字符串
            /// </summary>
            /// <param name="context">分割字符串</param>
            /// <param name="separator">new string[]{"."}</param>
            /// <returns>分割后的字段数组</returns>
            public static string[] StringSplit(string context, string[] separator)
            {
                string[] stringArray = null;
                stringArray = context.Split(separator, StringSplitOptions.None);
                return stringArray;
            }
            /// <summary>
            /// 检测字段中指定类型的字符数量
            /// </summary>
            /// <param name="context">完整字段</param>
            /// <param name="separator">符号</param>
            /// <returns>数量</returns>
            public static int CharCount(string context, char separator)
            {
                if (string.IsNullOrEmpty(context) || string.IsNullOrEmpty(separator.ToString()))
                {
                    throw new ArgumentNullException("charCount \n string invaild!");
                }
                int count = 0;
                for (int i = 0; i < context.Length; i++)
                {
                    if (context[i] == separator)
                    {
                        count++;
                    }
                }
                return count;
            }
            /// <summary>
            /// 匹配字符串，循环更快，适合需要处理大量字符串或重复多次操作的场景。
            /// </summary>
            /// <param name="context">传入的内容</param>
            /// <param name="word">检测的内容</param>
            /// <returns>匹配到的数量</returns>
            public static int SubstringCountg(string context, string word)
            {
                int count = 0;
                int index = 0;
                while ((index = context.IndexOf(word, index)) != -1)
                {
                    count++;
                    index += word.Length;
                }
                return count;
            }
            /// <summary>
            /// 匹配字符串，适合复杂的文本匹配或需要使用正则表达式的地方。
            /// </summary>
            /// <param name="context">传入的内容</param>
            /// <param name="word">检测的内容</param>
            /// <returns>匹配到的数量</returns>
            public static int SubstringCountWithRegex(string context, string word)
            {
                return Regex.Matches(context, Regex.Escape(word)).Count;
            }
            /// <summary>
            /// 获取内容在UTF8编码下的字节长度。
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
            public static bool StringContans(string context, string[] values)
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
            /// <summary>
            /// 是否为有效字段
            /// </summary>
            /// <param name="context">文本内容</param>
            /// <returns>是否有效</returns>
            public static bool IsStringValid(string context)
            {
                if (string.IsNullOrEmpty(context))
                    return false;
                return true;
            }
            /// <summary>
            /// 是否为有效字段，若无效，抛出异常
            /// </summary>
            /// <param name="context">文本内容</param>
            /// <param name="exceptionContext">抛出的异常文本</param>
            public static void IsStringValid(string context, string exceptionContext)
            {
                if (string.IsNullOrEmpty(context))
                    throw new ArgumentNullException(exceptionContext);
            }
            /// <summary>
            /// 多字符替换。
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
            /// <summary>
            /// 检测一段文本是否只为英文字母
            /// </summary>
            /// <param name="context">文本</param>
            /// <returns>是否只为英文字母</returns>
            public static bool IsLetterOnly(string context)
            {
                return context.All(char.IsLetter);
            }
            /// <summary>
            /// 检测一段文本是否为英文字母与数字
            /// </summary>
            /// <param name="context">文本</param>
            /// <returns>是否只为英文字母与数字</returns>
            public static bool IsLetterOrDigit(string context)
            {
                return context.All(char.IsLetterOrDigit);
            }
        }
    }
}
