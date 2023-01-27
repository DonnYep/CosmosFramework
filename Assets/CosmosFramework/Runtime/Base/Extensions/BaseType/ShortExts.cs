using System;

namespace Cosmos
{
    public static class ShortExts
    {
        /// <summary>
        /// 转换成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
