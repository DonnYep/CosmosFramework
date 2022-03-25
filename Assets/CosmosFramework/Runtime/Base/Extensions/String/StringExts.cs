using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public static class StringExts
    {
        /// <summary>
        /// 高级比较，可设定是否忽略大小写
        /// </summary>
        public static bool Contains(this string @this, string toCheck, StringComparison comp)
        {
            return @this.IndexOf(toCheck, comp) >= 0;
        }
        /// <summary>
        /// 是否含有中文
        /// </summary>
        public static bool IsContainChinese(this string @this)
        {
            bool flag = false;
            foreach (var a in @this)
            {
                if (a >= 0x4e00 && a <= 0x9fbb)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 是否为空字符串
        /// </summary>
        public static bool IsNullOrEmpty(this string @this)
        {
            return string.IsNullOrEmpty(@this);
        }
        /// <summary>
        /// string Base64加密
        /// </summary>
        public static string StringToBase64(this string @this)
        {
            var b = Encoding.Default.GetBytes(@this);
            return Convert.ToBase64String(b);
        }
        public static string Base64ToString(this string @this)
        {
            var b = Convert.FromBase64String(@this);
            return Encoding.Default.GetString(b);
        }
    }
}
