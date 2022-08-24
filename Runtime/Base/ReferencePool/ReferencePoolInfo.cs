using System;
using System.Runtime.InteropServices;
namespace Cosmos
{
    [StructLayout(LayoutKind.Auto)]
    public struct ReferencePoolInfo
    {
        readonly Type referenceType;
        /// <summary>
        /// 使用的数量；
        /// </summary>
        readonly int acquiredCount;
        /// <summary>
        /// 释放的数量；
        /// </summary>
        readonly int releasedCount;
        /// <summary>
        /// 池中存余的数量；
        /// </summary>
        readonly int poolAcquireCount;
        public Type ReferenceType { get { return referenceType; } }
        public int AcquiredCount { get { return acquiredCount; } }
        public int ReleasedCount { get { return releasedCount; } }
        public int PoolAcquireCount { get { return poolAcquireCount; } }
        public ReferencePoolInfo(Type referenceType, int acquiredCount, int releasedCount, int poolAcquireCount)
        {
            this.referenceType = referenceType;
            this.acquiredCount = acquiredCount;
            this.releasedCount = releasedCount;
            this.poolAcquireCount = poolAcquireCount;
        }
    }
}