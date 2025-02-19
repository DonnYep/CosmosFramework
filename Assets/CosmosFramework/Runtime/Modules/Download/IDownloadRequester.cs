using System;
 namespace  Cosmos.Download
{
    /// <summary>
    /// 下载请求器，用于下载前获取文件大小。
    /// </summary>
    public interface IDownloadRequester
    {
        /// <summary>
        /// 获取URI单个文件的大小。
        /// <para>若获取到，回调传入正确的数值，否则就传入-1。</para> 
        /// </summary>
        /// <param name="uri">统一资源名称</param>
        /// <param name="callback">回调</param>
        void GetUriFileSizeAsync(string uri, Action<long> callback);
        /// <summary>
        /// 获取多个URL地址下的所有文件的总和大小。
        /// <para>若获取到，回调传入正确的数值，否则就传入-1。</para> 
        /// </summary>
        /// <param name="uris">统一资源定位符</param>
        /// <param name="callback">回调</param>
        void GetUriFilesSizeAsync(string[] uris, Action<long> callback);
    }
}
