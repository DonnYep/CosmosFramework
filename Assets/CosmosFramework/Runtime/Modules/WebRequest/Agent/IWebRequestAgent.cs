using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 请求代理类接口，区分webclient与unityWebRequest；
    /// </summary>
    public interface IWebRequestAgent
    {
        /// <summary>
        /// 异步下载资源；
        /// 注意，返回值类型可以是Task与Coroutine任意一种表示异步的引用对象；
        /// </summary>
        /// <see cref="Task">Return vaalue</see>
        /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">传入的回调</param>
        /// <returns>表示异步的引用对象</returns>
        object DownloadAsync(string uri, WebRequestCallback webRequestCallback);
        /// <summary>
        /// 异步上传资源；
        /// 注意，返回值类型可以是Task与Coroutine任意一种表示异步的引用对象；
        /// </summary>
        /// <see cref="Task">Return vaalue</see>
        /// <see cref="UnityEngine. Coroutine">Return vaalue</see>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">传入的回调</param>
        /// <returns>表示异步的引用对象</returns>
        object UploadAsync(string uri, WebRequestCallback webRequestCallback);
    }
}
