using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    //================================================
    //1、下载代理帮助类，接口中所包含的方法的返回值类型理应是Task或者
    // Coroutine这类表示异步操作的状态对象。当前返回值类型为object，因为
    // object可以兼容类类型的异步操作对象；
    //2、接口中的下载方法目前只提供了单个文件的下载，多文件下载需要等待
    //当前下载任务完成后，由系统分配下一个任务循环下载；
    //================================================
    public interface IDownloadAgentHelper
    {
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
        IEnumerator DownloadFileAsync(DownloadTask downloadTask,object customeData);
        /// <summary>
        /// 异步下载资源；
        /// </summary>
        /// <see cref="FutureTask"></see>
        /// <param name="downloadTask">下载任务</param>
        /// <param name="startPosition">上次下载到的位置</param>
        /// <param name="customeData">用户自定义的数据</param>
        /// <returns>表示异步的引用对象</returns>
        IEnumerator DownloadFileAsync(DownloadTask downloadTask, long startPosition, object customeData);
    }
}
