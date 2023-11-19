using System;
namespace Cosmos.Download
{
    //================================================
    /*
     * 1、下载模块提供一键式资源下载、本地写入功能。通过监听开放的接口可
    * 以检测下载进度；
    * 
    * 2、载模块支持localhost文件下载与http地址文件的下载。模块下载到本
    *后，增量写入，以防下载错误导致的整体文件丢失。
    *
    *3、载模块支持断点续下。
    *
    *4、支持动态添加、移除、暂停、恢复下载任务；
    *
    *5、若不自定义设置下载器，(调用SetOrUpdateDownloadHelper方法)，则
    * 使用框架原生默认的下载器；
     */
    //================================================
    public interface IDownloadManager : IModuleManager, IModuleInstance
    {
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        event Action<DownloadStartEventArgs> OnDownloadStart;
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        event Action<DownloadSuccessEventArgs> OnDownloadSuccess;
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        event Action<DownloadFailureEventArgs> OnDownloadFailure;
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        event Action<DonwloadUpdateEventArgs> OnDownloadOverallProgress;
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        event Action<DownloadTasksCompletedEventArgs> OnAllDownloadTaskCompleted;
        /// <summary>
        /// 终止时删除下载中的文件
        /// </summary>
        bool DeleteFileOnAbort { get; set; }
        /// <summary>
        /// 任务过期时间，以秒为单位；
        /// </summary>
        int DownloadTimeout { get; set; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get; }
        /// <summary>
        /// 下载中的资源总数；
        /// </summary>
        int DownloadingCount { get; }

        /// <summary>
        /// 设置或更新downloader;
        /// </summary>
        /// <param name="newDownloader">下载器</param>
        void SetOrUpdateDownloadHelper(IDownloader newDownloader);
        /// <summary>
        /// 设置下载资源地址帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetUrlHelper(IDownloadUrlHelper helper);
        /// <summary>
        /// 设置资源请求帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetRequesterHelper(IDownloadRequester helper);
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="downloadUri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        /// <param name="downloadByteOffset">下载byte的偏移量，用于断点续传</param>
        /// <param name="downloadAppend">当本地存在时，下载时追加写入</param>
        /// <returns>下载序号</returns>
        long AddDownload(string downloadUri, string downloadPath, long downloadByteOffset = 0, bool downloadAppend = true);
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="downloadId">下载序号</param>
        /// <returns>移除结果</returns>
        bool RemoveDownload(long downloadId);
        /// <summary>
        /// 移除多个URI下载；
        /// </summary>
        /// <param name="downloadIds">下载序号集合</param>
        void RemoveDownloads(long[] downloadIds);
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        void RemoveAllDownload();
        /// <summary>
        /// 获取URI单个文件的大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="downloadUri">统一资源名称</param>
        /// <param name="callback">回调</param>
        void GetUriFileSizeAsync(string downloadUri, Action<long> callback);
        /// <summary>
        /// 获取一个URL地址下的所有文件的总和大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="downloadUrl">统一资源定位符</param>
        /// <param name="callback">回调</param>
        void GetUrlFilesSizeAsync(string downloadUrl, Action<long> callback);
        /// <summary>
        /// 设置完成下载配置后启动下载；
        void LaunchDownload();
        /// <summary>
        /// 取消下载
        /// </summary>
        void CancelDownload();
    }
}
