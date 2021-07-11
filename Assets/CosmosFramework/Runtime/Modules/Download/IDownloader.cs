using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    /// <summary>
    /// 下载模块下载器接口；
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// 下载开始事件；
        /// </summary>
        event Action<DownloadStartEventArgs> DownloadStart;
        /// <summary>
        /// 单个资源下载成功事件；
        /// </summary>
        event Action<DownloadSuccessEventArgs> DownloadSuccess;
        /// <summary>
        /// 单个资源下载失败事件；
        /// </summary>
        event Action<DownloadFailureEventArgs> DownloadFailure;
        /// <summary>
        /// 下载整体进度事件；
        /// </summary>
        event Action<DonwloadOverallEventArgs> DownloadOverall;
        /// <summary>
        /// 整体下载并写入完成事件
        /// </summary>
        event Action<DownloadAndWriteFinishEventArgs> DownloadAndWriteFinish;
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
        bool Downloading { get;  }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        int DownloadingCount { get;  }
        /// <summary>
        /// 添加URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="downloadPath">下载到地址的绝对路径</param>
        void AddUriDownload(string uri, string downloadPath);
        /// <summary>
        /// 移除URI下载；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        void RemoveUriDownload(string uri);
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        void RemoveAllDownload();
        /// <summary>
        /// 启动下载；
        /// </summary>
        void LaunchDownload();
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        void CancelDownload();
        /// <summary>
        /// 释放下载器；
        /// </summary>
        void Release();
    }
}
