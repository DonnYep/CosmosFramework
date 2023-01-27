using System;

namespace Cosmos
{
    public static class LongExts
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
