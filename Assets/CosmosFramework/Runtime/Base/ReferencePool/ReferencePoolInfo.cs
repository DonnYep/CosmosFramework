using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos
{
    public struct ReferencePoolInfo
    {
        readonly Type referenceType;
        /// <summary>
        /// 使用的数量；
        /// </summary>
        readonly int accquiredCount;
        /// <summary>
        /// 释放的数量；
        /// </summary>
        readonly int releasedCount;
        /// <summary>
        /// 池中存余的数量；
        /// </summary>
        readonly int poolAccquireCount;
        public Type ReferenceType { get { return referenceType; } }
        public int AccquiredCount { get { return accquiredCount; } }
        public int ReleasedCount { get { return releasedCount; } }
        public int PoolAccquireCount { get { return poolAccquireCount; } }
        public ReferencePoolInfo(Type referenceType, int accquiredCount, int releasedCount, int poolAccquireCount)
        {
            this.referenceType = referenceType;
            this.accquiredCount = accquiredCount;
            this.releasedCount = releasedCount;
            this.poolAccquireCount = poolAccquireCount;
        }
    }
}