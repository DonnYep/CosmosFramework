namespace Cosmos.Download
{
    public struct DownloadedData
    {
        /// <summary>
        /// 下载数据的构造；
        /// </summary>
        /// <param name="uri"> 资源定位地址</param>
        /// <param name="data">接收到的byte数据</param>
        /// <param name="downloadPath">下载后存储的路径</param>
        /// <param name="isCompleted">下载的是否是完整的数据</param>
        public DownloadedData(string uri, byte[] data, string downloadPath)
        {
            URI = uri;
            Data = data;
            DownloadPath = downloadPath;
        }
        /// <summary>
        /// 资源定位地址；
        /// </summary>
        public string URI { get; private set; }
        /// <summary>
        /// 接收到的byte数据；
        /// </summary>
        public byte[] Data { get; private set; }
        /// <summary>
        /// 下载后存储的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
    }
}
