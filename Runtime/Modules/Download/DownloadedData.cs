namespace Cosmos.Download
{
    public struct DownloadedData
    {
        /// <summary>
        /// 下载数据的构造；
        /// </summary>
        /// <param name="uri"> 资源定位地址</param>
        /// <param name="downloadPath">下载后存储的路径</param>
        public DownloadedData(string uri, string downloadPath)
        {
            URI = uri;
            DownloadPath = downloadPath;
        }
        /// <summary>
        /// 资源定位地址；
        /// </summary>
        public string URI { get; private set; }
        /// <summary>
        /// 下载后存储的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
    }
}
