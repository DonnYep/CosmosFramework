using System;
namespace Cosmos
{
    public static class IntExts
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        public static byte[] GetBytes(this int @this)
        {
            return BitConverter.GetBytes(@this);
        }
    }
}
