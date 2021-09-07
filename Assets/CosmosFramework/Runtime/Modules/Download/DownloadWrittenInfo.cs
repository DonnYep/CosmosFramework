using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    /// <summary>
    /// 数据写入的信息；
    /// </summary>
    internal struct DownloadWrittenInfo: IEquatable<DownloadWrittenInfo>
    {
        /// <summary>
        /// 已经缓存的长度；
        /// </summary>
        public long CachedLength { get; private set; }
        /// <summary>
        /// 已经写入持久化本地的长度；
        /// </summary>
        public long WrittenLength { get; private set; }
        public static DownloadWrittenInfo None { get { return new DownloadWrittenInfo(0, 0); } }
        /// <summary>
        /// 下载写入信息的构造函数；
        /// </summary>
        /// <param name="cachedLength">已经缓存的长度</param>
        /// <param name="writtenLength">已经写入持久化本地的长度</param>
        public DownloadWrittenInfo(long cachedLength, long writtenLength)
        {
            CachedLength = cachedLength;
            WrittenLength = writtenLength;
        }
        public override int GetHashCode()
        {
            return CachedLength.GetHashCode() ^ WrittenLength.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is DownloadWrittenInfo && Equals((DownloadWrittenInfo)obj);
        }
        public bool Equals(DownloadWrittenInfo other)
        {
            bool result = false;
            if (this.GetType() == other.GetType())
            {
                result = this.CachedLength.Equals(other.CachedLength) && this.WrittenLength.Equals(other.WrittenLength);
            }
            return result;
        }
    }
}
