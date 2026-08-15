using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源加载诊断系统。
    /// <para>记录所有资源加载行为，支持泄漏检测与运行时摘要输出，方便定位加载耗时、失败原因与资源泄漏。</para>
    /// <para>默认仅在编辑器开启；可通过 <see cref="SetEnabled"/> 在真机开启。</para>
    /// </summary>
    public static class ResourceDiagnostics
    {
        /// <summary>
        /// 记录上限，超出后覆盖最旧记录（环形）
        /// </summary>
        public const int MAX_RECORD_COUNT = 512;

        static bool enabled;
        static readonly List<ResourceLoadRecord> records = new List<ResourceLoadRecord>(MAX_RECORD_COUNT);
        static int failedCount;
        static int totalLoadCount;
        static int activeProviderCount;
        static bool captureStackTrace;

        /// <summary>
        /// 诊断是否开启
        /// </summary>
        public static bool Enabled
        {
            get { return enabled; }
        }
        /// <summary>
        /// 是否捕获调用栈（定位泄漏源头用，有少量性能开销）
        /// </summary>
        public static bool CaptureStackTrace
        {
            get { return captureStackTrace; }
            set { captureStackTrace = value; }
        }
        /// <summary>
        /// 历史加载记录（只读）
        /// </summary>
        public static IReadOnlyList<ResourceLoadRecord> Records
        {
            get { return records; }
        }
        /// <summary>
        /// 累计加载次数
        /// </summary>
        public static int TotalLoadCount
        {
            get { return totalLoadCount; }
        }
        /// <summary>
        /// 累计失败次数
        /// </summary>
        public static int FailedLoadCount
        {
            get { return failedCount; }
        }
        /// <summary>
        /// 当前激活的加载流程数量
        /// </summary>
        public static int ActiveProviderCount
        {
            get { return activeProviderCount; }
        }

        static ResourceDiagnostics()
        {
#if UNITY_EDITOR
            enabled = true;
#else
            enabled = false;
#endif
        }

        /// <summary>
        /// 开启或关闭诊断
        /// </summary>
        public static void SetEnabled(bool value)
        {
            enabled = value;
            if (!enabled)
                records.Clear();
        }
        /// <summary>
        /// 清空历史记录
        /// </summary>
        public static void ClearRecords()
        {
            records.Clear();
            totalLoadCount = 0;
            failedCount = 0;
        }
        /// <summary>
        /// 重置全部统计
        /// </summary>
        public static void Reset()
        {
            records.Clear();
            totalLoadCount = 0;
            failedCount = 0;
            activeProviderCount = 0;
        }

        #region 内部钩子（ProviderBase 调用）
        internal static void OnProviderStart(ProviderBase provider)
        {
            if (!enabled)
                return;
            totalLoadCount++;
            activeProviderCount++;
            var record = new ResourceLoadRecord()
            {
                AssetPath = provider.MainAssetInfo?.AssetPath,
                AssetName = provider.MainAssetInfo?.Name,
                BundleName = provider.MainAssetInfo?.BundleName,
                AssetTypeName = provider.AssetType?.Name,
                LoadMode = provider.LoadMode.ToString(),
                StartTimeMs = GetTimestampMs(),
                Succeeded = false,
                RefCount = provider.RefCount,
                StackTrace = captureStackTrace ? CaptureStack() : null,
            };
            AddRecord(record);
        }
        internal static void OnProviderFinish(ProviderBase provider, bool succeeded, string error)
        {
            if (!enabled)
                return;
            if (records.Count == 0)
                return;
            // 关联最近一条未结束的记录
            ResourceLoadRecord record = null;
            for (int i = records.Count - 1; i >= 0; i--)
            {
                var candidate = records[i];
                if (candidate.EndTimeMs == 0 && candidate.AssetPath == provider.MainAssetInfo?.AssetPath)
                {
                    record = candidate;
                    break;
                }
            }
            if (record == null)
                return;
            var endTime = GetTimestampMs();
            record.EndTimeMs = endTime;
            record.DurationSeconds = (endTime - record.StartTimeMs) / 1000f;
            record.Succeeded = succeeded;
            record.Error = error;
            record.RefCount = provider.RefCount;
            if (!succeeded)
                failedCount++;
        }
        internal static void OnProviderDestroy(ProviderBase provider)
        {
            if (!enabled)
                return;
            activeProviderCount--;
            if (activeProviderCount < 0)
                activeProviderCount = 0;
        }
        #endregion

        /// <summary>
        /// 输出诊断摘要文本（当前加载中的资产、包体、内存占用、失败统计）
        /// </summary>
        public static string GenerateReport()
        {
            var sb = new StringBuilder(512);
            sb.AppendLine($"== Cosmos.Resource Diagnostics ==");
            sb.AppendLine($"TotalLoadCount : {totalLoadCount}");
            sb.AppendLine($"FailedLoadCount : {failedCount}");
            sb.AppendLine($"ActiveProviderCount : {activeProviderCount}");
            sb.AppendLine($"RecordCount : {records.Count}");
            var package = CFResources.DefaultPackage;
            if (package != null)
            {
                sb.AppendLine($"LoadedBundleCount : {package.LoadedBundleCount}");
                sb.AppendLine($"LoadedProviderCount : {package.LoadedProviderCount}");
            }
            var leaked = GetLeakedRecords();
            if (leaked.Count > 0)
            {
                sb.AppendLine($"== Potential Leaks : {leaked.Count} ==");
                for (int i = 0; i < leaked.Count; i++)
                {
                    sb.AppendLine(leaked[i].ToString());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 最近失败的加载记录
        /// </summary>
        public static List<ResourceLoadRecord> GetFailedRecords()
        {
            var result = new List<ResourceLoadRecord>();
            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];
                if (record.EndTimeMs != 0 && !record.Succeeded)
                    result.Add(record);
            }
            return result;
        }

        /// <summary>
        /// 潜在泄漏记录：引用计数已归零但从未被释放过句柄的异常加载
        /// </summary>
        public static List<ResourceLoadRecord> GetLeakedRecords()
        {
            var result = new List<ResourceLoadRecord>();
            for (int i = 0; i < records.Count; i++)
            {
                var record = records[i];
                // 已完成但引用计数仍大于0，且时间已过去较久 —— 说明句柄可能未被释放
                if (record.EndTimeMs != 0 && record.Succeeded && record.RefCount > 0)
                    result.Add(record);
            }
            return result;
        }

        static void AddRecord(ResourceLoadRecord record)
        {
            if (records.Count >= MAX_RECORD_COUNT)
                records.RemoveAt(0);
            records.Add(record);
        }
        static long GetTimestampMs()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
        static string CaptureStack()
        {
            var stackTrace = new StackTrace(3, true);
            return stackTrace.ToString();
        }
    }
}
