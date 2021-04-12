using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
namespace Cosmos
{
    public static partial class Utility
    {
        /// <summary>
        /// 加密工具
        /// </summary>
        public static class Encryption
        {
            public  enum GUIDFormat
            {
                N,D,B,P,X
            }
            public static string GUID(GUIDFormat format)
            {
                return Guid.NewGuid().ToString(format.ToString());
            }
            /// <summary>
            /// MD5加密，返回16位加密后的大写16进制字符
            /// </summary>
            /// <param name="strData">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string MD5Encrypt16(string strData)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(strData);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] cryptString = md5.ComputeHash(md5Bytes);
                Utility.Text.StringBuilderCache.Clear();
                for (int i = 4; i < 12; i++)
                {
                    Utility.Text.StringBuilderCache.Append(cryptString[i].ToString("X2"));
                }
                return Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// MD5加密，返回32位加密后的大写16进制字符
            /// </summary>
            /// <param name="strData">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string MD5Encrypt32(string strData)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(strData);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] cryptString = md5.ComputeHash(md5Bytes);
                Utility.Text.StringBuilderCache.Clear();
                int length = cryptString.Length;
                for (int i = 0; i < length; i++)
                {
                    //X大写的16进制，x小写
                    Utility.Text.StringBuilderCache.Append(cryptString[i].ToString("X2"));
                }
                return Utility.Text.StringBuilderCache.ToString();
            }
            /// <summary>
            /// Base64加密，返回24位加密后的字符
            /// </summary>
            /// <param name="strData">需要加密的字符</param>
            /// <returns>加密后的结果</returns>
            public static string Base64Encrypt(string strData)
            {
                byte[] md5Bytes = Encoding.UTF8.GetBytes(strData);
                MD5 md5 = MD5.Create();
                byte[] cryptString = md5.ComputeHash(md5Bytes);
                return Convert.ToBase64String(cryptString);
            }
            /// <summary>
            /// DES加密
            /// </summary>
            /// <param name="strData">数据</param>
            /// <param name="strKey">密钥</param>
            /// <param name="iv">初向量</param>
            /// <returns>DES加密后的数据</returns>
            public static string DESEncrypt(string strData,string strKey,byte[] iv)
            {
                string strResult = "";
                try
                {
                    using (DESCryptoServiceProvider desc = new DESCryptoServiceProvider())
                    {
                        byte[] key = Encoding.UTF8.GetBytes(strKey);
                        byte[] data = Encoding.UTF8.GetBytes(strData);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, desc.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                            {
                                cs.Write(data, 0, data.Length);
                                cs.FlushFinalBlock();
                                strResult = Convert.ToBase64String(ms.ToArray());
                                return strResult;
                            }
                        }
                    }
                }
                catch 
                {
                    return strResult;
                }
            }
            /// <summary>
            /// 生成 8 位密钥
            /// Data Encryption Standard
            /// initialization vector
            /// </summary>
            /// <param name="key">需要生成的Key</param>
            /// <returns></returns>
            public static byte[] GenerateIV(string key)
            {
                var result = Encoding.UTF8.GetBytes(key);
                byte[] iv;
                if (result.Length > 8)
                {
                    iv = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        iv[i] = result[i];
                    }
                    return iv;
                }
                else if (result.Length < 8)
                {
                    iv = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        if (result.Length > i)
                            iv[i] = result[i];
                        else
                            iv[i] = 0;
                        return iv;
                    }
                }
                return result;
            }
            /// <summary>
            /// DES解密
            /// Data Encryption Standard
            /// </summary>
            /// <param name="encrpytedStr">被加密的数据</param>
            /// <param name="strKey">密钥</param>
            /// <returns></returns>
            public static string DESDecrypt(string encrpytedStr,string strKey,byte[] iv)
            {
                string strResut="";
                try
                {
                    using (DESCryptoServiceProvider desc=new DESCryptoServiceProvider())
                    {
                        byte[] key = Encoding.UTF8.GetBytes(strKey);
                        byte[] data = Convert.FromBase64String(encrpytedStr);
                        using (MemoryStream ms=new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, desc.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                            {
                                cs.Write(data, 0, data.Length);
                                cs.FlushFinalBlock();
                                return Encoding.UTF8.GetString(ms.ToArray());
                            }
                        }
                    }
                }
                catch 
                {
                    return strResut;
                }
            }
            /// <summary>
            /// 加密算法HmacSHA256  
            /// </summary>
            /// <param name="encrpytedStr">被加密的数据</param>
            /// <param name="strKey">加密密码</param>
            /// <returns>加密后的字段</returns>
            public static string HmacSHA256(string encrpytedStr, string strKey)
            {
                string encrpytedResult = string.Empty;
                using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(strKey)))
                {
                    byte[] hashMsg = mac.ComputeHash(Encoding.UTF8.GetBytes(encrpytedStr));
                    encrpytedResult = Convert.ToBase64String(hashMsg);
                }
                return encrpytedResult;
            }
        }
    }
}