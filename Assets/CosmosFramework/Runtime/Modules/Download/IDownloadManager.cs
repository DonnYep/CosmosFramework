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
        /// 下载模块配置；
        /// </summary>
        DownloadConfig DownloadConfig { get; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        bool Downloading { get; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        int DownloadableCount { get; }
        /// <summary>
        /// 下载模式；
        /// </summary>
        DownloaderMode DownloaderMode { get;  }
        /// <summary>
        /// 切换下载模式；
        /// 此操作为异步处理，当有个下载器正在下载时，等到下载器下载停止再切换；
        /// <see cref="Cosmos.Download. DownloaderMode"/>
        /// </summary>
        /// <param name="downloaderMode">下载模式</param>
        void SwitchDownloadMode(DownloaderMode downloaderMode);
        /// <summary>
        /// 设置下载配置；
        /// <see cref="Cosmos.Download. DownloadConfig"/>
        /// </summary>
        /// <param name="downloadConfig">下载配置</param>
        void SetDownloadConfig(DownloadConfig downloadConfig);
        /// <summary>
        /// 设置完成下载配置后启动下载；
        void LaunchDownload();
        /// <summary>
        /// 取消下载
        /// </summary>
        void CancelDownload();
        /// <summary>
        /// 重置下载配置‘
        /// </summary>
        void ResetDownloadConfig();
    }
}
