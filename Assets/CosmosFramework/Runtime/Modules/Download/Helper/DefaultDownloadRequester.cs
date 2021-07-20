using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.Download
{
    public class DefaultDownloadRequester : IDownloadRequester
    {
        /// <summary>
        /// 获取URI单个文件的大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="callback">回调</param>
        public void GetUriFileSizeAsync(string uri, Action<long> callback)
        {
            Utility.Unity.StartCoroutine(EnumGetFileSize(uri, callback));
        }
        /// <summary>
        /// 获取多个URL地址下的所有文件的总和大小；
        /// 若获取到，则回调传入正确的数值，否则就传入-1；
        /// </summary>
        /// <param name="url">统一资源定位符</param>
        /// <param name="callback">回调</param>
        public void GetUriFilesSizeAsync(string[] uris, Action<long> callback)
        {
            Utility.Unity.StartCoroutine(EnumGetMultiFilesSize(uris, callback));
        }
        IEnumerator EnumGetMultiFilesSize(string[] uris, Action<long> callback)
        {
            long overallSize = 0;
            var length = uris.Length;
            for (int i = 0; i < length; i++)
            {
                yield return EnumGetFileSize(uris[i], size =>
                {
                    if (size >= 0)
                        overallSize += size;
                });
            }
            callback?.Invoke(overallSize);
        }
        IEnumerator EnumGetFileSize(string uri, Action<long> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(uri))
            {
                yield return request.SendWebRequest();
                string size = request.GetResponseHeader("Content-Length");
                if (request.isNetworkError || request.isHttpError)
                    callback?.Invoke(-1);
                else
                    callback?.Invoke(Convert.ToInt64(size));
            }
        }
    }
}
