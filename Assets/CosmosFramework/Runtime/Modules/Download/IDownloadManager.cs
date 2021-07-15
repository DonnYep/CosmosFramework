using Cosmos.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public interface IDownloadManager : IModuleManager
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
        event Action<DonwloadOverallEventArgs> OnDownloadOverall;
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        event Action<DownloadAndWriteFinishEventArgs> OnDownloadAndWriteFinish;
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        bool DeleteFailureFile { get; set; }
        /// <summary>
        /// 任务过期时间，以秒为单位；
        /// </summary>
        float DownloadTimeout { get; set; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get; }
        /// <summary>
        /// 下载中的资源总数；
        /// </summary>
        int DownloadingCount { get; }
        /// <summary>
        /// 下载模式；
        /// </summary>
        DownloaderMode DownloaderMode { get;  }
        /// <summary>
        /// 可以用，但是没必要；
        /// 切换下载模式，切换下载器后会保留先前的的下载配置；
        /// 此操作为异步处理，当有个下载器正在下载时，等到下载器下载停止再切换；
        /// <see cref="Cosmos.Download. DownloaderMode"/>
        /// </summary>
        /// <param name="downloaderMode">下载模式</param>
        void SwitchDownloadMode(DownloaderMode downloaderMode);
        /// <summary>
        /// 设置下载资源地址帮助体；
        /// </summary>
        /// <param name="helper">帮助体对象</param>
        void SetIUrlHelper(IDownloadUrlHelper helper);
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        void AddUriDownload(string uri, string downloadPath);
        /// <summary>
        /// 将URL添加到下载队列，并下载当前URL页面下的所有文件到本地；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="downloadRootPath">下载到地址的根目录</param>
        void AddUrlDownload(string url, string downloadRootPath);
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        void RemoveUriDownload(string uri);
        /// <summary>
        /// 移除多个URI下载；
        /// </summary>
        /// <param name="uris">统一资源名称数组</param>
        void RemoveUrisDownload(string[] uris);
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        void RemoveAllDownload();
        /// <summary>
        /// 设置完成下载配置后启动下载；
        void LaunchDownload();
        /// <summary>
        /// 取消下载
        /// </summary>
        void CancelDownload();
    }
}
