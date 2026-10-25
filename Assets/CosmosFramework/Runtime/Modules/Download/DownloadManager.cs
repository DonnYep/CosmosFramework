using System;
using UnityEngine;

namespace Cosmos.Download
{
    //================================================
    /*
     * 1、下载模块与其它内置模块同级：Runtime/Modules/Download，同一程序集，
     *    因此可以直接使用 Utility.* / CosmosEntry / ReferencePool 等框架工具。
     *    不需要时：删除本目录 + BuiltinModuleRegistry 中的一行（其它核心模块不依赖它）。
     *
     * 2、调度与传输分离：DownloadScheduler（纯 C#，可单测）+ IDownloadTransport（默认 UnityWebRequest）。
     *
     * 3、本类只做三件事：把配置与公开 API 转发给调度器、把调度事件转成 EventArgs、
     *    在 OnUpdate 里用 unscaledTime 推进调度器（暂停菜单 timeScale=0 时下载不中断）。
     */
    //================================================
    [Module(Priority = ModuleConstant.PRIORITY_DOWNLOAD)]
    internal sealed class DownloadManager : Module, IDownloadManager
    {
        #region events
        Action<DownloadStartEventArgs> onDownloadStart;
        Action<DownloadSuccessEventArgs> onDownloadSuccess;
        Action<DownloadFailureEventArgs> onDownloadFailure;
        Action<DonwloadUpdateEventArgs> onDownloadOverallProgress;
        Action<DownloadTasksCompletedEventArgs> onAllDownloadTaskCompleted;
        ///<inheritdoc/>
        public event Action<DownloadStartEventArgs> OnDownloadStart
        {
            add { onDownloadStart += value; }
            remove { onDownloadStart -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadSuccessEventArgs> OnDownloadSuccess
        {
            add { onDownloadSuccess += value; }
            remove { onDownloadSuccess -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadFailureEventArgs> OnDownloadFailure
        {
            add { onDownloadFailure += value; }
            remove { onDownloadFailure -= value; }
        }
        ///<inheritdoc/>
        public event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress
        {
            add { onDownloadOverallProgress += value; }
            remove { onDownloadOverallProgress -= value; }
        }
        ///<inheritdoc/>
        public event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted
        {
            add { onAllDownloadTaskCompleted += value; }
            remove { onAllDownloadTaskCompleted -= value; }
        }
        #endregion
        DownloadSettings settings;
        DownloadScheduler scheduler;
        /// <summary>
        /// 上一次推进的 unscaledTime
        /// </summary>
        float lastTickUnscaledTime;
        ///<inheritdoc/>
        public DownloadSettings Settings { get { return settings; } }
        ///<inheritdoc/>
        public bool DeleteFileOnAbort
        {
            get { return scheduler.DeleteFileOnAbort; }
            set { scheduler.DeleteFileOnAbort = value; }
        }
        ///<inheritdoc/>
        public int DownloadTimeout
        {
            get { return (int)settings.TimeoutSeconds; }
            set { settings.TimeoutSeconds = value; }
        }
        ///<inheritdoc/>
        public int MaxConcurrentDownload
        {
            get { return settings.MaxConcurrent; }
            set { settings.MaxConcurrent = value; }
        }
        ///<inheritdoc/>
        public bool Downloading { get { return scheduler.Running; } }
        ///<inheritdoc/>
        public int DownloadingCount { get { return scheduler.PendingCount; } }
        ///<inheritdoc/>
        public void SetTransport(IDownloadTransport transport)
        {
            scheduler.SetTransport(transport);
        }
        ///<inheritdoc/>
        public long AddDownload(string downloadUrl, string downloadPath)
        {
            return AddDownload(downloadUrl, downloadPath, DownloadOptions.Default);
        }
        ///<inheritdoc/>
        public long AddDownload(string downloadUrl, string downloadPath, DownloadMode mode)
        {
            var options = DownloadOptions.Default;
            options.Mode = mode;
            return AddDownload(downloadUrl, downloadPath, options);
        }
        ///<inheritdoc/>
        public long AddDownload(string downloadUrl, string downloadPath, DownloadOptions options)
        {
            Utility.Text.IsStringValid(downloadUrl, "downloadUrl is invalid !");
            Utility.Text.IsStringValid(downloadPath, "downloadPath is invalid !");
            return scheduler.Add(downloadUrl, downloadPath, options);
        }
        ///<inheritdoc/>
        public bool RemoveDownload(long downloadId)
        {
            return scheduler.Remove(downloadId);
        }
        ///<inheritdoc/>
        public void RemoveDownloads(long[] downloadIds)
        {
            if (downloadIds == null)
                return;
            for (int i = 0; i < downloadIds.Length; i++)
                scheduler.Remove(downloadIds[i]);
        }
        ///<inheritdoc/>
        public void RemoveAllDownload()
        {
            scheduler.RemoveAll();
        }
        ///<inheritdoc/>
        public void LaunchDownload()
        {
            scheduler.Launch();
        }
        ///<inheritdoc/>
        public void PauseDownload()
        {
            scheduler.Pause();
        }
        ///<inheritdoc/>
        public void ResumeDownload()
        {
            scheduler.Resume();
        }
        ///<inheritdoc/>
        public void CancelDownload()
        {
            scheduler.RemoveAll();
        }
        ///<inheritdoc/>
        public bool GetDownloadInfo(long downloadId, out DownloadInfo downloadInfo)
        {
            downloadInfo = default;
            if (!scheduler.TryGetTask(downloadId, out var task))
                return false;
            downloadInfo = task.ToInfo();
            return true;
        }
        ///<inheritdoc/>
        public int GetDownloadTaskCount()
        {
            return scheduler.TaskCount;
        }
        protected override void OnInitialization()
        {
            settings = new DownloadSettings();
            scheduler = new DownloadScheduler(new UnityWebRequestDownloadTransport(), settings);
            scheduler.TaskStarted += HandleTaskStarted;
            scheduler.TaskProgress += HandleTaskProgress;
            scheduler.TaskCompleted += HandleTaskCompleted;
            scheduler.TaskFailed += HandleTaskFailed;
            scheduler.BatchCompleted += HandleBatchCompleted;
            lastTickUnscaledTime = Time.unscaledTime;
        }
        /// <summary>
        /// 用 unscaledTime 差值推进：timeScale=0 的暂停菜单下下载仍会继续
        /// </summary>
        protected override void OnUpdate()
        {
            //没有任务时不做任何事（空队列不产生每帧开销）
            if (scheduler.TaskCount == 0)
                return;
            var currentUnscaledTime = Time.unscaledTime;
            var deltaTime = currentUnscaledTime - lastTickUnscaledTime;
            lastTickUnscaledTime = currentUnscaledTime;
            if (deltaTime <= 0f)
                return;
            scheduler.Tick(deltaTime);
        }
        protected override void OnTermination()
        {
            if (scheduler == null)
                return;
            scheduler.TaskStarted -= HandleTaskStarted;
            scheduler.TaskProgress -= HandleTaskProgress;
            scheduler.TaskCompleted -= HandleTaskCompleted;
            scheduler.TaskFailed -= HandleTaskFailed;
            scheduler.BatchCompleted -= HandleBatchCompleted;
            scheduler.RemoveAll();
        }
        /// <summary>
        /// 当前任务序号（从1开始）：已完成数 + 1，用于给事件提供"第几个"
        /// </summary>
        /// <returns>序号</returns>
        int GetCurrentTaskIndex()
        {
            var taskCount = scheduler.TaskCount;
            var pending = scheduler.PendingCount;
            var index = taskCount - pending + 1;
            return index < 1 ? 1 : index;
        }
        void HandleTaskStarted(DownloadTask task)
        {
            if (onDownloadStart == null)
                return;
            var eventArgs = DownloadStartEventArgs.Create(task.ToInfo(), GetCurrentTaskIndex(), scheduler.TaskCount);
            onDownloadStart.Invoke(eventArgs);
            DownloadStartEventArgs.Release(eventArgs);
        }
        void HandleTaskProgress(DownloadTask task)
        {
            if (onDownloadOverallProgress == null)
                return;
            var eventArgs = DonwloadUpdateEventArgs.Create(task.ToInfo(), GetCurrentTaskIndex(), scheduler.TaskCount, TimeSpan.FromSeconds(0d),
                scheduler.OverallProgress, scheduler.TotalDownloadedBytes, scheduler.TotalExpectedBytes);
            onDownloadOverallProgress.Invoke(eventArgs);
            DonwloadUpdateEventArgs.Release(eventArgs);
        }
        void HandleTaskCompleted(DownloadTask task)
        {
            if (onDownloadSuccess == null)
                return;
            var eventArgs = DownloadSuccessEventArgs.Create(task.ToInfo(), GetCurrentTaskIndex(), scheduler.TaskCount, TimeSpan.Zero);
            onDownloadSuccess.Invoke(eventArgs);
            DownloadSuccessEventArgs.Release(eventArgs);
        }
        void HandleTaskFailed(DownloadTask task)
        {
            if (onDownloadFailure == null)
                return;
            var eventArgs = DownloadFailureEventArgs.Create(task.ToInfo(), GetCurrentTaskIndex(), scheduler.TaskCount, task.Error, TimeSpan.Zero);
            onDownloadFailure.Invoke(eventArgs);
            DownloadFailureEventArgs.Release(eventArgs);
        }
        void HandleBatchCompleted(System.Collections.Generic.List<DownloadTask> successed, System.Collections.Generic.List<DownloadTask> failed, float elapsedSeconds)
        {
            if (onAllDownloadTaskCompleted == null)
                return;
            var successedInfos = new DownloadInfo[successed.Count];
            for (int i = 0; i < successed.Count; i++)
                successedInfos[i] = successed[i].ToInfo();
            var failedInfos = new DownloadInfo[failed.Count];
            for (int i = 0; i < failed.Count; i++)
                failedInfos[i] = failed[i].ToInfo();
            var eventArgs = DownloadTasksCompletedEventArgs.Create(successedInfos, failedInfos, TimeSpan.FromSeconds(elapsedSeconds), successed.Count + failed.Count);
            onAllDownloadTaskCompleted.Invoke(eventArgs);
            DownloadTasksCompletedEventArgs.Release(eventArgs);
        }
    }
}
