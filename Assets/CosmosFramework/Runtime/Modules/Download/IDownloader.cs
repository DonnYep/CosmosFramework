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
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get;  }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        int DownloadableCount { get;  }
        /// <summary>
        /// 为下载器进行配置；
        /// </summary>
        /// <param name="downloadConfig">下载配置数据</param>
        void SetDownloadConfig(DownloadConfig downloadConfig);
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
