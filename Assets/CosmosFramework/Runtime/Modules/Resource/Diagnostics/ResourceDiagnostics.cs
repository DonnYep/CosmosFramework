using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源加载诊断。
    /// <para>记录与具体 Provider 无关的信息：谁在什么时候加载了什么、耗时多少、是否失败、有没有忘记释放。</para>
    /// </summary>
    public static class ResourceDiagnostics
    {
        /// <summary>
        /// 历史记录上限（环形裁剪）
        /// </summary>
        public const int MAX_RECORDS = 512;

        static bool enabled = UnityEngine.Application.isEditor;
        static bool captureStackTrace;
        static readonly List<ResourceLoadRecord> records = new List<ResourceLoadRecord>(MAX_RECORDS);
        static readonly Dictionary<ResourceProviderOperation, ResourceLoadRecord> activeRecords = new Dictionary<ResourceProviderOperation, ResourceLoadRecord>();
        static int totalLoadCount;
        static int failedLoadCount;

        /// <summary>
        /// 是否开启
        /// </summary>
        public static bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        /// <summary>
        /// 是否捕获调用栈
        /// </summary>
        public static bool CaptureStackTrace
        {
            get { return captureStackTrace; }
            set { captureStackTrace = value; }
        }
        /// <summary>
        /// 当前激活的操作数
        /// </summary>
        public static int ActiveOperationCount
        {
            get { return activeRecords.Count; }
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
            get { return failedLoadCount; }
        }
        /// <summary>
        /// 历史记录
        /// </summary>
        public static IReadOnlyList<ResourceLoadRecord> Records
        {
            get { return records; }
        }

        internal static void OnOperationStart(ResourceProviderOperation operation)
        {
            if (!enabled || operation == null)
                return;
            totalLoadCount++;
            var record = new ResourceLoadRecord()
            {
                Key = operation.AssetInfo != null ? operation.AssetInfo.Key : string.Empty,
                ProviderName = operation.Provider != null ? operation.Provider.Name : string.Empty,
                AssetTypeName = operation.AssetType != null ? operation.AssetType.Name : string.Empty,
                LoadMode = operation.LoadMode.ToString(),
                StartTime = DateTime.Now,
                Succeeded = false,
            };
            if (captureStackTrace)
                record.StackTrace = new StackTrace(2, true).ToString();
            activeRecords[operation] = record;
        }

        internal static void OnOperationFinish(ResourceProviderOperation operation)
        {
            if (operation == null)
                return;
            if (!activeRecords.TryGetValue(operation, out var record))
            {
                if (!enabled)
                    return;
                record = new ResourceLoadRecord()
                {
                    Key = operation.AssetInfo != null ? operation.AssetInfo.Key : string.Empty,
                    ProviderName = operation.Provider != null ? operation.Provider.Name : string.Empty,
                    AssetTypeName = operation.AssetType != null ? operation.AssetType.Name : string.Empty,
                    LoadMode = operation.LoadMode.ToString(),
                };
            }
            activeRecords.Remove(operation);
            if (!enabled)
                return;
            record.DurationSeconds = operation.LoadDurationSeconds;
            record.Succeeded = operation.Status == OperationStatus.Succeeded;
            record.Error = operation.Error;
            if (!record.Succeeded)
                failedLoadCount++;
            records.Add(record);
            if (records.Count > MAX_RECORDS)
                records.RemoveRange(0, records.Count - MAX_RECORDS);
        }

        /// <summary>
        /// 操作被提前释放（未完成即销毁）：清掉活跃记录，避免活跃表无限增长。
        /// </summary>
        internal static void OnOperationDestroyed(ResourceProviderOperation operation)
        {
            if (operation == null)
                return;
            activeRecords.Remove(operation);
        }

        /// <summary>
        /// 清空记录。
        /// </summary>
        public static void Clear()
        {
            records.Clear();
            activeRecords.Clear();
            totalLoadCount = 0;
            failedLoadCount = 0;
        }

        /// <summary>
        /// 获取失败记录。
        /// </summary>
        public static List<ResourceLoadRecord> GetFailedRecords()
        {
            var result = new List<ResourceLoadRecord>();
            for (int i = 0; i < records.Count; i++)
            {
                if (!records[i].Succeeded)
                    result.Add(records[i]);
            }
            return result;
        }

        /// <summary>
        /// 获取潜在泄漏记录：仍处于加载中且引用计数大于 0 的操作。
        /// <para>长时间停留在此列表的通常是句柄未释放或 Complete 回调未触发。</para>
        /// </summary>
        public static List<ResourceLoadRecord> GetLeakedRecords()
        {
            var result = new List<ResourceLoadRecord>();
            foreach (var kv in activeRecords)
            {
                if (kv.Key != null && kv.Key.RefCount > 0)
                    result.Add(kv.Value);
            }
            return result;
        }

        /// <summary>
        /// 生成诊断摘要。
        /// </summary>
        public static string GenerateReport()
        {
            var builder = new StringBuilder(1024);
            builder.AppendLine("========== Resource Diagnostics ==========");
            builder.AppendLine($"total load : {totalLoadCount}    failed : {failedLoadCount}    active : {activeRecords.Count}    records : {records.Count}");
            if (records.Count == 0)
            {
                builder.AppendLine("(no record)");
                return builder.ToString();
            }
            var slowest = new List<ResourceLoadRecord>(records);
            slowest.Sort((a, b) => b.DurationSeconds.CompareTo(a.DurationSeconds));
            builder.AppendLine("---- slowest 10 ----");
            var slowCount = Math.Min(10, slowest.Count);
            for (int i = 0; i < slowCount; i++)
                builder.AppendLine(slowest[i].ToString());

            var failed = GetFailedRecords();
            if (failed.Count > 0)
            {
                builder.AppendLine($"---- failed ({failed.Count}) ----");
                var failCount = Math.Min(30, failed.Count);
                for (int i = 0; i < failCount; i++)
                    builder.AppendLine(failed[i].ToString());
            }
            var leaked = GetLeakedRecords();
            if (leaked.Count > 0)
            {
                builder.AppendLine($"---- active / potential leak ({leaked.Count}) ----");
                var leakCount = Math.Min(30, leaked.Count);
                for (int i = 0; i < leakCount; i++)
                    builder.AppendLine(leaked[i].ToString());
            }
            builder.AppendLine("==========================================");
            return builder.ToString();
        }
    }
}
