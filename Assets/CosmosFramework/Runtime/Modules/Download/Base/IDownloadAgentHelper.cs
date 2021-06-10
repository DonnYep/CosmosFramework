using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public interface IDownloadAgentHelper
    {
        event Action<DownloadStartEventArgs> DownloadStart;
        event Action<DownloadSuccessEventArgs> DownloadUpdate;
        event Action<DownloadSuccessEventArgs> DownloadSuccess;
        event Action<DownloadFailureEventArgs> DownloadFailure;
        /// <summary>
        /// 异步下载资源；
        /// FutureTask结构体是作为Wrapper将异步对象进行地址缓存；
        /// </summary>
        /// <param name="uri">资源路径</param>
        /// <param name="customeData">用户自定义的数据</param>
        /// <returns>表示异步的引用对象</returns>
       object DownloadFileAsync(string uri,object customeData);
        /// <summary>
        /// 异步下载资源；
        /// FutureTask结构体是作为Wrapper将异步对象进行地址缓存；
        /// </summary>
        /// <see cref="FutureTask"></see>
        /// <param name="uri">资源路径</param>
        /// <param name="startPosition">上次下载到的位置</param>
        /// <param name="customeData">用户自定义的数据</param>
        /// <returns>表示异步的引用对象</returns>
        object DownloadFileAsync(string uri,long startPosition, object customeData);
    }
}
