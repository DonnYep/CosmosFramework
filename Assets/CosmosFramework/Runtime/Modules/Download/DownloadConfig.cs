using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class DownloadConfig
    {
        /// <summary>
        /// Remote资源所在的地址；
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// Local资源下载后存储的地址；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// URL的相对地址文件列表；
        /// 若URL根目录为http://127.0.0.1:80/res/，文件地址为http://127.0.0.1:80/res/test.txt； 则test.txt即为FileList中的地址；
        /// </summary>
        public string[] FileList { get; private set; }
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile { get; private set; }
        /// <summary>
        /// 任务过期时间，以秒为单位；
        /// </summary>
        public float DownloadTimeout { get; private set; }
        /// <summary>
        /// 下载配置构造函数；
        /// </summary>
        /// <param name="url">Remote资源所在的地址</param>
        /// <param name="downloadPath"> Local资源下载后存储的地址</param>
        /// <param name="fileList">URL的相对地址文件列表</param>
        /// <param name="deleteFailureFile">是否删除本地下载失败的文件</param>
        /// <param name="timeout">任务过期时间，以秒为单位</param>
        public DownloadConfig(string url, string downloadPath,string[] fileList, bool deleteFailureFile,  float timeout)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is invalid !");
            if (string.IsNullOrEmpty(downloadPath))
                throw new ArgumentNullException("DonwloadPath is invalid !");
            if (fileList == null)
                throw new ArgumentNullException("FileList is invalid");
            URL = url;
            DownloadPath = downloadPath;
            DeleteFailureFile = deleteFailureFile;
            FileList = fileList;
            if (timeout < 0)
                DownloadTimeout = 0;
            else
                DownloadTimeout = timeout;
        }
        /// <summary>
        /// 下载配置构造函数；
        /// </summary>
        /// <param name="url">Remote资源所在的地址</param>
        /// <param name="downloadPath"> Local资源下载后存储的地址</param>
        /// <param name="fileList">URL的相对地址文件列表</param>
        public DownloadConfig(string url, string downloadPath, string[] fileList):this(url,downloadPath,fileList,false,0){}
        /// <summary>
        /// 重置配置；
        /// </summary>
        public void Reset()
        {
            URL = string.Empty;
            DownloadPath = string.Empty;
            DownloadTimeout = 0;
            DeleteFailureFile = false;
        }
    }
}
