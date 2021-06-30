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
    //1、多文件下载器用于下载复数文件，内部核心组件为单文件下载多次循环
    //下载pending的文件。文件下载成功、失败、下载中、下载开始都带有委托
    //事件；
    //2、下载器成功下载到一个文件后，会将这个成功的文件写入本地。若下载
    //中断，则保留当前写入的信息，下次连接网络时继续下载；
    //================================================
    /// <summary>
    /// 多文件下载器；
    /// </summary>
    public class MultiFileDownloader
    {
        /// <summary>
        /// 这里的事件都是单个文件的进程状态；
        /// </summary>
        #region events
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        Action<DonwloadOverallEventArgs> downloadOverall;
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
        #endregion

        List<string> pendingURIs = new List<string>();
        List<string> completedURIs = new List<string>();
        List<string> failureURIs = new List<string>();
        Queue<DownloadedData> downloadedDataQueues = new Queue<DownloadedData>();

        public string DownloadPath { get; set; }
        public bool Downloading { get; set; }
        public bool DeleteFailureFile { get; set; }
        DateTime startTime;
        DateTime endTime;
        int timeout;
        public int DownloadCount { get; private set; }

        bool stopDownload;
        /// <summary>
        /// key=> uri ; value=>fileName ;
        /// </summary>
        Dictionary<string, string> uriNameDict;

        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float unitResRatio;

        int currentDownloadIndex= 0;

        public bool Download(Dictionary<string, string> uriNameDict)
        {
            this.uriNameDict = uriNameDict;
            if (uriNameDict == null)
                throw new ArgumentNullException("UriNameDict  is invalid !");
            pendingURIs.AddRange(uriNameDict.Keys);
            DownloadCount = uriNameDict.Count;
            unitResRatio= 100f / DownloadCount;
            if (pendingURIs.Count == 0 || Downloading)
                return false;
            Downloading = true;
            startTime = DateTime.Now;
            RecursiveDownload();
            return true;
        }
        public void CancelDownload()
        {
            stopDownload = true;
        }
        async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                endTime = DateTime.Now;
                return;
            }
            string uri = pendingURIs[0];
            currentDownloadIndex = DownloadCount - pendingURIs.Count;
            var fileName = uriNameDict[uri];
            var fileDownloadPath = Path.Combine(DownloadPath, fileName);
            pendingURIs.RemoveAt(0);
            if (!stopDownload)
            {
                await DownloadWebRequest(uri, fileDownloadPath, null);
                RecursiveDownload();
            }
        }
        IEnumerator DownloadWebRequest(string uri, string fileDownloadPath, object customeData)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                if (timeout > 0)
                    request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, fileDownloadPath, customeData);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    var individualPercentage =request.downloadProgress* 100;
                    ProcessOverallProgress(uri, DownloadPath, request.downloadProgress,individualPercentage, customeData);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var successEventArgs = DownloadSuccessEventArgs.Create(request.url, fileDownloadPath, request.downloadHandler.data, customeData);
                        downloadSuccess?.Invoke(successEventArgs);
                        ProcessOverallProgress(uri, DownloadPath, 1,100, customeData);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        completedURIs.Add(uri);
                        uriNameDict.Remove(uri);
                        downloadedDataQueues.Enqueue(new DownloadedData(request.downloadHandler.data, fileDownloadPath));
                    }
                }
                else
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(request.url, fileDownloadPath, request.error, customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
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
        /// <param name="customeData">自定义用户数据</param>
        void ProcessOverallProgress(string uri,string downloadPath,float overallPercent,float individualPercent, object customeData)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / (float)DownloadCount);
            var overallProgress =overallIndexPercent + (unitResRatio* overallPercent);
            var eventArgs = DonwloadOverallEventArgs.Create(uri, downloadPath, overallProgress, individualPercent, customeData);
            downloadOverall.Invoke(eventArgs);
            DonwloadOverallEventArgs.Release(eventArgs);
        }
        [TickRefresh]
        async void TickRefresh()
        {
            if (downloadedDataQueues.Count > 0)
            {
                var data = downloadedDataQueues.Dequeue();
                await Task.Run(() =>
                 {
                     try
                     {
                         Utility.IO.WriteFile(data.Data, data.DownloadPath);
                     }
                     catch
                     {
                         Utility.Debug.LogError(data.DownloadPath);
                     }
                 });
            }
        }
    }
}
