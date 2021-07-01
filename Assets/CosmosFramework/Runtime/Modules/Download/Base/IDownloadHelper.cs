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

        event Action DownloadStart;
        event Action<string> DownloadFailure;
        event Action<byte[]> DownloadSuccess;
        event Action<float> DownloadUpdate;

        /// <summary>
        /// 异步下载资源；
        /// </summary>
        /// <param name="downloadTask">下载任务</param>
        /// <returns>表示异步的引用对象</returns>
        Task DownloadFileAsync(DownloadInfo downloadTask);
        /// <summary>
        /// 异步下载资源；
        /// </summary>
        /// <see cref="FutureTask"></see>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">上次下载到的位置</param>
        /// <returns>表示异步的引用对象</returns>
        Task DownloadFileAsync(DownloadInfo downloadTask, long startPosition);
        /// <summary>
        /// 停止当前正在下载的任务；
        /// 默认会缓存已经下载到的进度，便于下次断点续下；
        /// </summary>
        /// <param name="clearDownloadedFile">是否清理已经下载的的文件</param>
        void CancelDownload(bool clearDownloadedFile = false);
        /// <summary>
        /// 清理监听的时间；
        /// </summary>
        void ClearEvents();
    }
}
