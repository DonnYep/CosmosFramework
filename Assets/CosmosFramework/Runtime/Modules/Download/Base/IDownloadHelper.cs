using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    //================================================
    //1、下载帮助体只实现单文件下载，多文件下载需由downloader调度完成；
    //================================================
    public interface IDownloadHelper
    {
        /// <summary>
        /// 当前下载helper是否正在下载；
        /// </summary>
        bool Downloading { get; }
        event Action<DownloadStartEventArgs> DownloadStart;
        event Action<DownloadUpdateEventArgs> DownloadUpdate;
        event Action<DownloadSuccessEventArgs> DownloadSuccess;
        event Action<DownloadFailureEventArgs> DownloadFailure;
        /// <summary>
        /// 异步下载资源；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="customeData">用户自定义的数据</param>
        /// <returns>表示异步的引用对象</returns>
        void DownloadFileAsync(DownloadInfo downloadTask, object customeData);
        /// <summary>
        /// 异步下载资源；
        /// </summary>
        /// <see cref="FutureTask"></see>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">上次下载到的位置</param>
        /// <param name="customeData">用户自定义的数据</param>
        /// <returns>表示异步的引用对象</returns>
        void DownloadFileAsync(DownloadInfo downloadTask, long startPosition, object customeData);
        /// <summary>
        /// 停止当前正在下载的任务；
        /// 默认会缓存已经下载到的进度，便于下次断点续下；
        /// </summary>
        /// <param name="clearDownloadedFile">是否清理已经下载的的文件</param>
        void CancelDownload(bool clearDownloadedFile = false);
    }
}
