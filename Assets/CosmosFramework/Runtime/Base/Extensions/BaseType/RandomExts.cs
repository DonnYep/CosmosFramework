using System;
namespace Cosmos
{
    public static class RandomExts
    {
        /// <summary>
        /// 生成真正的随机数
        /// </summary>
        public static int StrictNext(this Random @this, int maxValue = int.MaxValue)
        {
            return new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)).Next(maxValue);
        }
    }
}
