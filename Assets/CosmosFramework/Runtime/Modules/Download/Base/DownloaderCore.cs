using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Cosmos.Download
{
    //================================================
    //1、多文件下载器用于下载复数文件。文件下载成功、失败、下载中、下载
    //开始、下载结束都带有委托事件；
    //2、下载器成功下载到一个文件后，会将这个成功的文件写入本地。若下载
    //中断，则保留当前写入的信息，下次连接网络时继续下载；
    //================================================
    /// <summary>
    /// 文件下载器；
    /// </summary>
    public class DownloaderCore
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

        List<string> pendingURIs = new List<string>();
        List<string> successURIs = new List<string>();
        List<string> failureURIs = new List<string>();

        Queue<DownloadedData> downloadedDataQueue = new Queue<DownloadedData>();

        DateTime downloadStartTime;
        DateTime downloadEndTime;

        UnityWebRequest unityWebRequest;
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float unitResRatio;
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;

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
            RecursiveDownload();
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
            failureURIs.AddRange(pendingURIs);
            pendingURIs.Clear();
            var eventArgs = DownloadFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            downloadFinish?.Invoke(eventArgs);
            DownloadFinishEventArgs.Release(eventArgs);
            unityWebRequest?.Abort();
            canDownload = false;
        }
        async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                downloadEndTime = DateTime.Now;
                var eventArgs = DownloadFinishEventArgs.Create(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
                downloadFinish?.Invoke(eventArgs);
                DownloadFinishEventArgs.Release(eventArgs);
                canDownload = false;
                Downloading = false;
                return;
            }
            string downloadableUri = pendingURIs[0];
            currentDownloadIndex = DownloadableCount - pendingURIs.Count;
            var fileDownloadPath = Path.Combine(DownloadPath, downloadableUri);
            pendingURIs.RemoveAt(0);
            if (canDownload)
            {
                var remoteUri = Utility.IO.WebPathCombine(URL, downloadableUri);
                await DownloadWebRequest(remoteUri, fileDownloadPath);
                RecursiveDownload();
            }
        }
        IEnumerator DownloadWebRequest(string uri, string fileDownloadPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                unityWebRequest = request;
                if (DownloadTimeout > 0)
                    request.timeout = DownloadTimeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, fileDownloadPath);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    var individualProgress = request.downloadProgress * 100;
                    ProcessOverallProgress(uri, DownloadPath, request.downloadProgress, individualProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var successEventArgs = DownloadSuccessEventArgs.Create(request.url, fileDownloadPath, request.downloadHandler.data);
                        downloadSuccess?.Invoke(successEventArgs);
                        ProcessOverallProgress(uri, DownloadPath, 1, 100);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successURIs.Add(uri);
                        downloadedDataQueue.Enqueue(new DownloadedData(request.downloadHandler.data, fileDownloadPath));
                    }
                }
                else
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(request.url, fileDownloadPath, request.error);
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
