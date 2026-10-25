using System;
using System.Collections.Generic;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载调度器：任务状态机 + 并发控制 + 失败重试（指数退避）+ 断点续传统一校验。
    /// <para>本类是纯 C#（不依赖 UnityEngine）：由模块在 OnUpdate 里调用 <see cref="Tick"/> 推进，
    /// 传输层通过 <see cref="IDownloadTransport"/> 注入，因此可以在无 Unity 环境下直接做单元测试。</para>
    /// <para>恢复策略：</para>
    /// <para>1、数据始终写入临时文件，长度即断点偏移（跨进程可用）；</para>
    /// <para>2、续传时若服务端忽略 Range（返回 200 而不是 206），丢弃临时文件并从 0 重下，绝不追加脏数据；</para>
    /// <para>3、传输完成后校验文件长度，不符则按"偏短可续、偏长重下"处理，校验通过才移动到目标路径。</para>
    /// </summary>
    internal sealed class DownloadScheduler
    {
        readonly List<DownloadTask> tasks = new List<DownloadTask>();
        readonly List<DownloadTask> completedBatch = new List<DownloadTask>();
        readonly List<DownloadTask> failedBatch = new List<DownloadTask>();
        IDownloadTransport transport;
        readonly DownloadSettings settings;
        long downloadId;
        float elapsedTime;
        float batchStartTime;
        bool started;
        bool paused;
        int batchTotal;
        int batchHandled;

        /// <summary>
        /// 调度参数
        /// </summary>
        public DownloadSettings Settings { get { return settings; } }
        /// <summary>
        /// 是否处于"已启动、批次未结束"状态
        /// </summary>
        public bool Running { get { return started; } }
        /// <summary>
        /// 是否已暂停
        /// </summary>
        public bool Paused { get { return paused; } }
        /// <summary>
        /// 未结束的任务数（排队/下载中/等待重试/暂停）
        /// </summary>
        public int PendingCount
        {
            get
            {
                var count = 0;
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (!tasks[i].IsHandled)
                        count++;
                }
                return count;
            }
        }
        /// <summary>
        /// 正在传输的任务数
        /// </summary>
        public int ActiveCount
        {
            get
            {
                var count = 0;
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (tasks[i].State == DownloadState.Downloading)
                        count++;
                }
                return count;
            }
        }
        /// <summary>
        /// 队列中的任务总数（含已完成的）
        /// </summary>
        public int TaskCount { get { return tasks.Count; } }
        /// <summary>
        /// 已下载总字节
        /// </summary>
        public long TotalDownloadedBytes
        {
            get
            {
                long total = 0L;
                for (int i = 0; i < tasks.Count; i++)
                    total += tasks[i].DownloadedBytes > 0 ? tasks[i].DownloadedBytes : 0L;
                return total;
            }
        }
        /// <summary>
        /// 预期总字节（仅统计已知长度的任务）
        /// </summary>
        public long TotalExpectedBytes
        {
            get
            {
                long total = 0L;
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (tasks[i].ExpectedBytes > 0)
                        total += tasks[i].ExpectedBytes;
                }
                return total;
            }
        }
        /// <summary>
        /// 整体进度 0~1（按字节加权；长度未知时返回 0）
        /// </summary>
        public float OverallProgress
        {
            get
            {
                var expected = TotalExpectedBytes;
                if (expected <= 0L)
                    return 0f;
                var progress = (float)TotalDownloadedBytes / expected;
                return progress < 0f ? 0f : (progress > 1f ? 1f : progress);
            }
        }
        /// <summary>
        /// 取消任务时是否删除未完成的临时文件
        /// </summary>
        public bool DeleteFileOnAbort { get; set; }

        /// <summary>
        /// 单个任务开始传输
        /// </summary>
        public event Action<DownloadTask> TaskStarted;
        /// <summary>
        /// 单个任务进度（已按 <see cref="DownloadSettings.ProgressIntervalSeconds"/> 节流）
        /// </summary>
        public event Action<DownloadTask> TaskProgress;
        /// <summary>
        /// 单个任务完成
        /// </summary>
        public event Action<DownloadTask> TaskCompleted;
        /// <summary>
        /// 单个任务失败（重试耗尽）
        /// </summary>
        public event Action<DownloadTask> TaskFailed;
        /// <summary>
        /// 整批下载结束：成功列表、失败列表、耗时（秒）
        /// </summary>
        public event Action<List<DownloadTask>, List<DownloadTask>, float> BatchCompleted;

        /// <summary>
        /// 构造调度器
        /// </summary>
        /// <param name="transport">传输实现</param>
        /// <param name="settings">调度参数</param>
        public DownloadScheduler(IDownloadTransport transport, DownloadSettings settings)
        {
            if (transport == null)
                throw new ArgumentNullException("transport is invalid !");
            if (settings == null)
                throw new ArgumentNullException("settings is invalid !");
            this.transport = transport;
            this.settings = settings;
        }
        /// <summary>
        /// 替换传输实现；已在进行中的传输会被中止并按断点续传重新创建
        /// </summary>
        /// <param name="newTransport">新的传输实现</param>
        public void SetTransport(IDownloadTransport newTransport)
        {
            if (newTransport == null)
                throw new ArgumentNullException("transport is invalid !");
            transport = newTransport;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.State != DownloadState.Downloading)
                    continue;
                if (task.Transfer != null)
                {
                    task.Transfer.Cancel();
                    task.Transfer = null;
                }
                //回到排队，下一次派发会用新传输从临时文件长度处续传
                task.State = DownloadState.Queued;
            }
        }
        /// <summary>
        /// 加入一个下载任务
        /// </summary>
        /// <param name="downloadUrl">资源地址</param>
        /// <param name="downloadPath">目标绝对路径</param>
        /// <param name="options">任务参数</param>
        /// <returns>下载Id（从1开始递增）</returns>
        public long Add(string downloadUrl, string downloadPath, DownloadOptions options)
        {
            var tempPath = DownloadStorage.GetTempPath(downloadPath, settings.TempFileExtension);
            DownloadStorage.EnsureDirectory(tempPath);
            var task = new DownloadTask(++downloadId, downloadUrl, downloadPath, tempPath, options, settings);
            tasks.Add(task);
            if (started)
                batchTotal++;
            return task.DownloadId;
        }
        /// <summary>
        /// 按Id取任务
        /// </summary>
        /// <param name="downloadId">下载Id</param>
        /// <param name="task">任务</param>
        /// <returns>是否存在</returns>
        public bool TryGetTask(long downloadId, out DownloadTask task)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].DownloadId == downloadId)
                {
                    task = tasks[i];
                    return true;
                }
            }
            task = null;
            return false;
        }
        /// <summary>
        /// 移除任务（正在下载的会立即中止）
        /// </summary>
        /// <param name="downloadId">下载Id</param>
        /// <returns>是否移除成功</returns>
        public bool Remove(long downloadId)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.DownloadId != downloadId)
                    continue;
                AbortTask(task);
                tasks.RemoveAt(i);
                if (started && !task.IsHandled)
                    batchTotal--;
                if (DeleteFileOnAbort)
                    DownloadStorage.Delete(task.TempPath);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 移除全部任务（不触发批次完成事件）
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                AbortTask(tasks[i]);
                if (DeleteFileOnAbort)
                    DownloadStorage.Delete(tasks[i].TempPath);
            }
            tasks.Clear();
            completedBatch.Clear();
            failedBatch.Clear();
            started = false;
            paused = false;
            batchTotal = 0;
            batchHandled = 0;
        }
        /// <summary>
        /// 启动当前批次
        /// </summary>
        public void Launch()
        {
            if (started)
                return;
            started = true;
            paused = false;
            batchTotal = tasks.Count;
            batchHandled = 0;
            batchStartTime = elapsedTime;
            completedBatch.Clear();
            failedBatch.Clear();
            for (int i = 0; i < tasks.Count; i++)
            {
                if (!tasks[i].IsHandled)
                    tasks[i].State = DownloadState.Queued;
            }
        }
        /// <summary>
        /// 暂停：中止进行中的传输（临时文件保留），不再派发新任务
        /// </summary>
        public void Pause()
        {
            paused = true;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.State != DownloadState.Downloading)
                    continue;
                task.PauseRequested = true;
                if (task.Transfer != null)
                {
                    task.Transfer.Cancel();
                    task.Transfer = null;
                }
                task.State = DownloadState.Paused;
            }
        }
        /// <summary>
        /// 恢复：被暂停的任务重新排队（从临时文件长度处续传）
        /// </summary>
        public void Resume()
        {
            paused = false;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.State == DownloadState.Paused)
                {
                    task.PauseRequested = false;
                    task.State = DownloadState.Queued;
                }
            }
        }
        /// <summary>
        /// 每帧推进
        /// </summary>
        /// <param name="deltaTime">推进时间（秒）</param>
        public void Tick(float deltaTime)
        {
            if (deltaTime < 0f)
                deltaTime = 0f;
            elapsedTime += deltaTime;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.State == DownloadState.Downloading)
                {
                    TickTransfer(task);
                }
                else if (task.State == DownloadState.WaitingRetry)
                {
                    task.RetryCountdown -= deltaTime;
                    if (task.RetryCountdown <= 0f)
                        task.State = DownloadState.Queued;
                }
            }
            if (!paused)
                Dispatch();
            if (started && batchHandled >= batchTotal)
                FinishBatch();
        }
        /// <summary>
        /// 派发排队任务，直到并发槽用满
        /// </summary>
        void Dispatch()
        {
            var capacity = settings.MaxConcurrent - ActiveCount;
            while (capacity-- > 0)
            {
                var next = FindNextQueued();
                if (next == null)
                    return;
                StartTask(next);
            }
        }
        /// <summary>
        /// 取优先级最高的排队任务（同优先级按入队顺序）
        /// </summary>
        DownloadTask FindNextQueued()
        {
            DownloadTask next = null;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.State != DownloadState.Queued)
                    continue;
                if (next == null || task.Priority > next.Priority)
                    next = task;
            }
            return next;
        }
        /// <summary>
        /// 开始一次尝试
        /// </summary>
        /// <param name="task">任务</param>
        void StartTask(DownloadTask task)
        {
            task.StartOffset = ResolveStartOffset(task);
            task.DownloadedBytes = task.StartOffset;
            task.PauseRequested = false;
            task.Error = null;
            task.State = DownloadState.Downloading;
            task.LastProgressTime = elapsedTime;
            task.LastProgressBytes = task.StartOffset;
            if (task.StartTime == default(DateTime))
                task.StartTime = DateTime.Now;
            var request = new DownloadTransferRequest()
            {
                DownloadUrl = task.DownloadUrl,
                TempPath = task.TempPath,
                StartOffset = task.StartOffset,
                TimeoutSeconds = task.TimeoutSeconds,
                Priority = task.Priority,
            };
            IDownloadTransfer transfer = null;
            try
            {
                transfer = transport.CreateTransfer(request);
            }
            catch (Exception e)
            {
                FailAttempt(task, $"create transfer failed : {e.Message}");
                return;
            }
            if (transfer == null)
            {
                FailAttempt(task, "create transfer failed : transport returned null");
                return;
            }
            task.Transfer = transfer;
            TaskStarted?.Invoke(task);
        }
        /// <summary>
        /// 计算本次尝试的起始偏移
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns>起始字节偏移</returns>
        long ResolveStartOffset(DownloadTask task)
        {
            switch (task.Mode)
            {
                case DownloadMode.Overwrite:
                    DownloadStorage.Delete(task.TempPath);
                    return 0L;
                case DownloadMode.Append:
                    return task.AppendOffset > 0L ? task.AppendOffset : 0L;
                default:
                    var length = task.GetTempLength();
                    //残留的"看起来已完整"临时文件不可信（可能是上一次失败前就写满的脏数据），直接重下
                    if (task.ExpectedBytes > 0L && length >= task.ExpectedBytes)
                    {
                        DownloadStorage.Delete(task.TempPath);
                        return 0L;
                    }
                    return length;
            }
        }
        /// <summary>
        /// 推进一次传输并处理其结束
        /// </summary>
        /// <param name="task">任务</param>
        void TickTransfer(DownloadTask task)
        {
            var transfer = task.Transfer;
            if (transfer == null)
            {
                FailAttempt(task, "transfer is null");
                return;
            }
            transfer.Tick();
            UpdateProgress(task, transfer);
            switch (transfer.State)
            {
                case DownloadTransferState.Running:
                    return;
                case DownloadTransferState.Succeeded:
                    task.Transfer = null;
                    CompleteTransfer(task, transfer);
                    return;
                case DownloadTransferState.Canceled:
                    task.Transfer = null;
                    task.State = task.PauseRequested ? DownloadState.Paused : DownloadState.Canceled;
                    return;
                default:
                    task.Transfer = null;
                    FailAttempt(task, string.IsNullOrEmpty(transfer.Error) ? "download failed" : transfer.Error);
                    return;
            }
        }
        /// <summary>
        /// 更新进度与速度（进度事件按配置节流）
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="transfer">传输</param>
        void UpdateProgress(DownloadTask task, IDownloadTransfer transfer)
        {
            var totalBytes = transfer.TotalBytes;
            if (totalBytes > 0L)
            {
                //206 的 Content-Length 是剩余长度，需要加上偏移才是文件总长
                task.ExpectedBytes = transfer.ResponseCode == 206 ? totalBytes + task.StartOffset : totalBytes;
            }
            task.DownloadedBytes = task.StartOffset + (transfer.DownloadedBytes > 0L ? transfer.DownloadedBytes : 0L);
            if (task.ExpectedBytes > 0L && task.DownloadedBytes > task.ExpectedBytes)
                task.DownloadedBytes = task.ExpectedBytes;
            var interval = settings.ProgressIntervalSeconds;
            var elapsed = elapsedTime - task.LastProgressTime;
            if (interval > 0f && elapsed < interval)
                return;
            var deltaBytes = task.DownloadedBytes - task.LastProgressBytes;
            task.SpeedBytesPerSecond = elapsed > 0f ? (long)(deltaBytes / elapsed) : 0L;
            task.LastProgressTime = elapsedTime;
            task.LastProgressBytes = task.DownloadedBytes;
            TaskProgress?.Invoke(task);
        }
        /// <summary>
        /// 传输成功：校验 Range / 文件长度，通过后提交
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="transfer">传输</param>
        void CompleteTransfer(DownloadTask task, IDownloadTransfer transfer)
        {
            task.State = DownloadState.Verifying;
            if (transfer.ResponseCode != 206 && task.StartOffset > 0)
            {
                //服务端忽略了 Range：此时临时文件里是"旧片段 + 完整文件"，必须丢弃重下
                DownloadStorage.Delete(task.TempPath);
                task.ExpectedBytes = -1L;
                FailAttempt(task, $"server ignored Range header (HTTP {transfer.ResponseCode})");
                return;
            }
            var fileLength = task.GetTempLength();
            if (task.ExpectedBytes > 0L && fileLength != task.ExpectedBytes)
            {
                //偏长说明写脏了，删除；偏短保留，下次续传
                if (fileLength > task.ExpectedBytes)
                    DownloadStorage.Delete(task.TempPath);
                FailAttempt(task, $"file size mismatch : {fileLength} / {task.ExpectedBytes}");
                return;
            }
            try
            {
                DownloadStorage.Commit(task.TempPath, task.DownloadPath);
            }
            catch (Exception e)
            {
                FailAttempt(task, $"commit file failed : {e.Message}");
                return;
            }
            task.DownloadedBytes = fileLength;
            task.ExpectedBytes = fileLength;
            task.EndTime = DateTime.Now;
            task.SpeedBytesPerSecond = 0L;
            task.State = DownloadState.Completed;
            batchHandled++;
            completedBatch.Add(task);
            TaskCompleted?.Invoke(task);
        }
        /// <summary>
        /// 一次尝试失败：重试（指数退避）或判定失败
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="error">失败原因</param>
        void FailAttempt(DownloadTask task, string error)
        {
            task.AttemptCount++;
            task.Error = error;
            if (!paused && task.AttemptCount <= task.RetryLimit)
            {
                //比预期长的文件是脏数据，留着只会越叠越多
                if (task.ExpectedBytes > 0L && task.GetTempLength() > task.ExpectedBytes)
                    DownloadStorage.Delete(task.TempPath);
                task.State = DownloadState.WaitingRetry;
                var factor = Math.Pow(2d, task.AttemptCount - 1);
                task.RetryCountdown = (float)(settings.RetryDelaySeconds * factor);
                return;
            }
            task.EndTime = DateTime.Now;
            task.SpeedBytesPerSecond = 0L;
            task.State = DownloadState.Failed;
            batchHandled++;
            failedBatch.Add(task);
            TaskFailed?.Invoke(task);
        }
        /// <summary>
        /// 中止任务上的传输
        /// </summary>
        /// <param name="task">任务</param>
        void AbortTask(DownloadTask task)
        {
            task.PauseRequested = false;
            if (task.Transfer != null)
            {
                task.Transfer.Cancel();
                task.Transfer = null;
            }
            if (task.State == DownloadState.Downloading)
                task.State = DownloadState.Canceled;
        }
        /// <summary>
        /// 批次结束：清理并派发完成事件
        /// </summary>
        void FinishBatch()
        {
            started = false;
            paused = false;
            var success = new List<DownloadTask>(completedBatch);
            var failed = new List<DownloadTask>(failedBatch);
            var elapsed = elapsedTime - batchStartTime;
            tasks.Clear();
            completedBatch.Clear();
            failedBatch.Clear();
            batchTotal = 0;
            batchHandled = 0;
            BatchCompleted?.Invoke(success, failed, elapsed);
        }
    }
}
