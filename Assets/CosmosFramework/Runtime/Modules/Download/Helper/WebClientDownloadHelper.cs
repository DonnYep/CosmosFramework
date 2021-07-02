using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;

namespace Cosmos.Download
{
    public class WebClientDownloadHelper : IDownloader
    {
        #region events
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        Action<DonwloadOverallEventArgs> downloadOverall;
        Action<DownloadFinishEventArgs> downloadFinish;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public event Action<DonwloadOverallEventArgs> DownloadOverall
        {
            add { downloadOverall += value; }
            remove { downloadOverall -= value; }
        }
        public event Action<DownloadFinishEventArgs> DownloadFinish
        {
            add { downloadFinish += value; }
            remove { downloadFinish -= value; }
        }
        #endregion

        /// <summary>
        /// 下载到本地的路径；
        /// </summary>
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 是否删除本地下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile { get; set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadableCount { get; private set; }
        /// <summary>
        /// 资源的地址；
        /// </summary>
        public string URL { get; private set; }
        /// <summary>
        /// 下载过期时间；
        /// </summary>
        public int DownloadTimeout { get; private set; }

        WebClient webClient;

        List<string> pendingURIs = new List<string>();
        List<string> successURIs = new List<string>();
        List<string> failureURIs = new List<string>();

        Queue<DownloadedData> downloadedDataQueue = new Queue<DownloadedData>();

        DateTime downloadStartTime;
        DateTime downloadEndTime;

        bool canDownload;
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float unitResRatio;
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadIndex = 0;

        /// <summary>
        /// 异步下载；
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="downloadPath">下载到本地的地址</param>
        /// <param name="downloadableList">可下载的文件列表</param>
        /// <param name="timeout">文件下载过期时间</param>
        public void Download(string url, string downloadPath, string[] downloadableList, int timeout = 0)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is invalid !");
            if (string.IsNullOrEmpty(downloadPath))
                throw new ArgumentNullException("DonwloadPath is invalid !");
            if (downloadableList == null)
                throw new ArgumentNullException("Downloadable is invalid !");
            DownloadPath = downloadPath;
            if (timeout <= 0)
                this.DownloadTimeout = 0;
            else
                this.DownloadTimeout = timeout;
            canDownload = true;
            URL = url;
            pendingURIs.AddRange(downloadableList);
            DownloadableCount = downloadableList.Length;
            unitResRatio = 100f / DownloadableCount;
            if (pendingURIs.Count == 0 || !canDownload)
                return;
            Downloading = true;
            downloadStartTime = DateTime.Now;
        }
        /// <summary>
        /// 下载轮询，需要由外部调用；
        /// </summary>
        public async void TickRefresh()
        {
            if (!canDownload)
                return;
            if (downloadedDataQueue.Count > 0)
            {
                var data = downloadedDataQueue.Dequeue();
                await Task.Run(() =>
                {
                    try
                    {
                        Utility.IO.WriteFile(data.Data, data.DownloadPath);
                    }
                    catch { }
                });
            }
        }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        public void AbortDownload()
        {

        }
        void WebRequest(string uri, string fileDownloadPath)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Downloading = true;
                    this.webClient = webClient;
                    var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                    downloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                    webClient.DownloadDataTaskAsync(uri);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        ProcessOverallProgress(uri, DownloadPath, (float)eventArgs.ProgressPercentage / 100, eventArgs.ProgressPercentage);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        Downloading = false;
                        var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, eventArgs.Result);
                        downloadSuccess?.Invoke(successEventArgs);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successURIs.Add(uri);
                        downloadedDataQueue.Enqueue(new DownloadedData(eventArgs.Result, fileDownloadPath));
                    };
                }
                catch (Exception exception)
                {
                    var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, exception.ToString());
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
                    ProcessOverallProgress(uri, DownloadPath, 1, 100);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }
        }
        /// <summary>
        /// 处理整体进度；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="overallPercent">整体百分比进度</param>
        /// <param name="individualPercent">资源个体百分比</param>
        void ProcessOverallProgress(string uri, string downloadPath, float overallPercent, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / (float)DownloadableCount);
            var overallProgress = overallIndexPercent + (unitResRatio * overallPercent);
            var eventArgs = DonwloadOverallEventArgs.Create(uri, downloadPath, overallProgress, individualPercent);
            downloadOverall.Invoke(eventArgs);
            DonwloadOverallEventArgs.Release(eventArgs);
        }
    }
}
