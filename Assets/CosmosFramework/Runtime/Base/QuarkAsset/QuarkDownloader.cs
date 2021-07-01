using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Cosmos.Quark
{
    public class QuarkDownloader
    {
        Action<string, string> downloadFailure;
        Action<string, float, float> downloadOverall;
        Action<string, byte[]> downloadSuccess;
        Action<string> downloadStart;
        public event Action<string, string> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public event Action<string, float, float> DownloadOverall
        {
            add { downloadOverall += value; }
            remove { downloadOverall -= value; }
        }
        public event Action<string, byte[]> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<string> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public bool Downloading { get; private set; }
        public int DownloadCount { get; private set; }
        public string DownloadPath { get; private set; }
        /// <summary>
        /// 删除下载失败的文件；
        /// </summary>
        public bool DeleteFailureFile { get; set; }
        public string URL { get; private set; }
        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float unitResRatio;
        int currentDownloadIndex = 0;

        List<string> pendingURIs = new List<string>();
        List<string> completedURIs = new List<string>();
        List<string> failureURIs = new List<string>();
        Queue<QuarkDownloadData> downloadedDataQueues = new Queue<QuarkDownloadData>();
        bool stopDownload;
        public void Download(string url, string[] downloadableList)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is invalid !");
            if (downloadableList == null)
                throw new ArgumentNullException("Downloadable is invalid !");
            URL = url;
            pendingURIs.AddRange(downloadableList);
            DownloadCount = downloadableList.Length;
            unitResRatio = 100f / DownloadCount;
            if (pendingURIs.Count == 0 || Downloading)
                return;
            Downloading = true;
            RecursiveDownload();
        }
        public void CancelDownload()
        {
            stopDownload = true;
        }
        async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                return;
            }
            string downloadableUri = pendingURIs[0];
            currentDownloadIndex = DownloadCount - pendingURIs.Count;
            var fileDownloadPath = Utility.IO.PathCombine(DownloadPath, downloadableUri);
            pendingURIs.RemoveAt(0);
            if (!stopDownload)
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
                downloadStart?.Invoke(uri);
                Downloading = true;
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    var individualProgress = request.downloadProgress * 100;
                    ProcessOverallProgress(uri, request.downloadProgress, individualProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        ProcessOverallProgress(uri, 1, 100);
                        downloadSuccess.Invoke(uri, request.downloadHandler.data);
                        completedURIs.Add(uri);
                    }
                }
                else
                {
                    Downloading = false;
                    downloadFailure?.Invoke(uri, request.error);
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
        /// <param name="overallPercent">整体百分比进度</param>
        /// <param name="individualPercent">资源个体百分比</param>
        void ProcessOverallProgress(string uri, float overallPercent, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / DownloadCount);
            var overallProgress = overallIndexPercent + (unitResRatio * overallPercent);
            downloadOverall?.Invoke(uri, overallPercent, individualPercent);
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
