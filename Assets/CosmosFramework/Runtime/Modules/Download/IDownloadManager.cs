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
        event Action<DownloadStartEventArgs> DownloadStart;
        event Action<DownloadSuccessEventArgs> DownloadSuccess;
        event Action<DownloadFailureEventArgs> DownloadFailure;
        event Action<DonwloadOverallEventArgs> DownloadOverall;
        event Action<DownloadFinishEventArgs> DownloadFinish;
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
