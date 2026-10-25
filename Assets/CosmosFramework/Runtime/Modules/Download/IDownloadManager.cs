using System;

namespace Cosmos.Download
{
    //================================================
    /*
     * 1、下载模块与其它内置模块同级（Runtime/Modules/Download，同一程序集）：
     *    直接使用 Utility.* / CosmosEntry / ReferencePool 等框架工具，不需要任何跨程序集约定。
     *    若项目完全用不到：删除本目录 + BuiltinModuleRegistry 中的一行即可（其它核心模块不依赖它）。
     *
     * 2、支持批量队列、并发上限、优先级、暂停/恢复、取消、失败重试（指数退避）。
     *
     * 3、支持断点续传：数据先写临时文件，长度即断点偏移；跨进程可用（重装/重启后再次 Add 同一目标即可续传）。
     *    续传时服务端必须返回 206；若服务端忽略 Range 返回 200，会被识别为"脏数据"并自动从 0 重下。
     *
     * 4、下载完成后校验文件长度，通过才从临时文件移动到目标路径（避免半截文件被当成可用资源）。
     *
     * 5、传输层可替换：IDownloadTransport，默认实现基于 UnityWebRequest。
     */
    //================================================
    public interface IDownloadManager : IModuleManager
    {
        /// <summary>
        /// 单个任务开始下载事件
        /// </summary>
        event Action<DownloadStartEventArgs> OnDownloadStart;
        /// <summary>
        /// 单个任务下载成功事件
        /// </summary>
        event Action<DownloadSuccessEventArgs> OnDownloadSuccess;
        /// <summary>
        /// 单个任务下载失败事件（重试耗尽）
        /// </summary>
        event Action<DownloadFailureEventArgs> OnDownloadFailure;
        /// <summary>
        /// 下载进度事件（已按配置节流）
        /// </summary>
        event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress;
        /// <summary>
        /// 整批任务完成事件（成功与失败列表）
        /// </summary>
        event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted;
        /// <summary>
        /// 调度参数（并发、重试、超时、进度节流、临时文件后缀）
        /// </summary>
        DownloadSettings Settings { get; }
        /// <summary>
        /// 取消任务时是否删除未完成的临时文件
        /// <para>默认 false：保留临时文件以便下次续传</para>
        /// </summary>
        bool DeleteFileOnAbort { get; set; }
        /// <summary>
        /// 单次尝试的超时时间（秒），&lt;=0 表示不限
        /// </summary>
        int DownloadTimeout { get; set; }
        /// <summary>
        /// 最大并发下载数
        /// </summary>
        int MaxConcurrentDownload { get; set; }
        /// <summary>
        /// 是否处于"已启动且批次未结束"状态
        /// </summary>
        bool Downloading { get; }
        /// <summary>
        /// 未结束的任务数（排队/下载中/等待重试/暂停）
        /// </summary>
        int DownloadingCount { get; }
        /// <summary>
        /// 替换传输实现（默认 <see cref="UnityWebRequestDownloadTransport"/>）
        /// </summary>
        /// <param name="transport">传输实现</param>
        void SetTransport(IDownloadTransport transport);
        /// <summary>
        /// 加入下载任务（断点续传模式）
        /// </summary>
        /// <param name="downloadUrl">资源地址</param>
        /// <param name="downloadPath">目标绝对路径</param>
        /// <returns>下载Id（从1开始）</returns>
        long AddDownload(string downloadUrl, string downloadPath);
        /// <summary>
        /// 加入下载任务
        /// </summary>
        /// <param name="downloadUrl">资源地址</param>
        /// <param name="downloadPath">目标绝对路径</param>
        /// <param name="mode">写入模式</param>
        /// <returns>下载Id</returns>
        long AddDownload(string downloadUrl, string downloadPath, DownloadMode mode);
        /// <summary>
        /// 加入下载任务
        /// </summary>
        /// <param name="downloadUrl">资源地址</param>
        /// <param name="downloadPath">目标绝对路径</param>
        /// <param name="options">任务参数</param>
        /// <returns>下载Id</returns>
        long AddDownload(string downloadUrl, string downloadPath, DownloadOptions options);
        /// <summary>
        /// 移除任务（进行中的会立即中止，是否删除临时文件取决于 <see cref="DeleteFileOnAbort"/>）
        /// </summary>
        /// <param name="downloadId">下载Id</param>
        /// <returns>是否移除成功</returns>
        bool RemoveDownload(long downloadId);
        /// <summary>
        /// 移除多个任务
        /// </summary>
        /// <param name="downloadIds">下载Id集合</param>
        void RemoveDownloads(long[] downloadIds);
        /// <summary>
        /// 移除全部任务（不触发整批完成事件）
        /// </summary>
        void RemoveAllDownload();
        /// <summary>
        /// 启动当前批次
        /// </summary>
        void LaunchDownload();
        /// <summary>
        /// 暂停：中止进行中的传输（保留临时文件），不再派发新任务
        /// </summary>
        void PauseDownload();
        /// <summary>
        /// 恢复：暂停的任务重新排队，并从临时文件长度处续传
        /// </summary>
        void ResumeDownload();
        /// <summary>
        /// 取消整批下载（等价于 <see cref="RemoveAllDownload"/>）
        /// </summary>
        void CancelDownload();
        /// <summary>
        /// 获取任务快照
        /// </summary>
        /// <param name="downloadId">下载Id</param>
        /// <param name="downloadInfo">任务快照</param>
        /// <returns>是否存在</returns>
        bool GetDownloadInfo(long downloadId, out DownloadInfo downloadInfo);
        /// <summary>
        /// 当前队列中的任务数（含已完成未清理的批次）
        /// </summary>
        /// <returns>任务数</returns>
        int GetDownloadTaskCount();
    }
}
