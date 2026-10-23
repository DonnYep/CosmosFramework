using UnityEngine;

namespace Cosmos.Resource
{
    /// <summary>
    /// 包体加载预算。
    /// <para>限制单帧发起的 AssetBundle 异步加载数量，避免同一帧内大量请求造成 IO/主线程尖峰。</para>
    /// <para>以 <see cref="Time.frameCount"/> 自动按帧重置，无需外部驱动。</para>
    /// </summary>
    internal static class ResourceLoadBudget
    {
        static int maxConcurrentBundleLoads = 8;
        static int frame = -1;
        static int usedCount;

        /// <summary>
        /// 单帧最多发起的包体加载数量；小于等于 0 表示不限制
        /// </summary>
        public static int MaxConcurrentBundleLoads
        {
            get { return maxConcurrentBundleLoads; }
            set { maxConcurrentBundleLoads = value; }
        }
        /// <summary>
        /// 尝试占用一个本帧的加载配额
        /// </summary>
        /// <returns>是否拿到配额；未拿到时应推迟到下一帧</returns>
        public static bool TryAcquire()
        {
            if (maxConcurrentBundleLoads <= 0)
                return true;
            var currentFrame = Time.frameCount;
            if (currentFrame != frame)
            {
                frame = currentFrame;
                usedCount = 0;
            }
            if (usedCount >= maxConcurrentBundleLoads)
                return false;
            usedCount++;
            return true;
        }
        /// <summary>
        /// 重置预算（资源系统重新初始化时调用）
        /// </summary>
        public static void Reset()
        {
            frame = -1;
            usedCount = 0;
        }
    }
}
