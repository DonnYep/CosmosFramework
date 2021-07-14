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
        class ResponseData
        {
            public ResponseData(string uri, byte[] data, string downloadPath)
            {
                URI = uri;
                Data = data;
                DownloadPath = downloadPath;
            }
            public string URI { get; private set; }
            public byte[] Data { get; private set; }
            public string DownloadPath { get; private set; }
        }
        class ResponseWriteInfo
        {
            /// <summary>
            /// 已经缓存的长度；
            /// </summary>
            public long CachedLength { get; private set; }
            /// <summary>
            /// 已经写入持久化本地的长度；
            /// </summary>
            public long WrittenLength { get; private set; }
            public ResponseWriteInfo(long cachedLength, long writtenLength)
            {
                CachedLength = cachedLength;
                WrittenLength = writtenLength;
            }
        }
        #region events
        Action<string, string> downloadStart;
        Action<string, string, byte[]> downloadSuccess;
        Action<string, string, string> downloadFailure;
        Action<string, string, float, float> downloadOverall;
        Action<string[], string[], TimeSpan> downloadFinish;
        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public event Action<string, string> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---Data
        /// </summary>
        public event Action<string, string, byte[]> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public event Action<string, string, string> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public event Action<string, string, float, float> DownloadOverall
        {
            add { downloadOverall += value; }
            remove { downloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public event Action<string[], string[], TimeSpan> DownloadFinish
        {
            add { downloadFinish += value; }
            remove { downloadFinish -= value; }
        }
        #endregion
        public string DownloadPath { get; private set; }
        public string URL { get; private set; }
        public int DownloadTimeout { get; private set; }
        public bool DeleteFailureFile { get; set; }

        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 下载中的资源总数；
        /// </summary>
        public int DownloadingCount { get { return pendingURIs.Count; } }

        List<string> pendingURIs = new List<string>();
        List<string> successURIs = new List<string>();
        List<string> failureURIs = new List<string>();

        /// <summary>
        /// URI===[[缓存的长度===写入本地的长度]]；
        /// 数据写入记录；
        /// </summary>
        Dictionary<string, ResponseWriteInfo> dataWriteDict = new Dictionary<string, ResponseWriteInfo>();
        DateTime downloadStartTime;
        DateTime downloadEndTime;

        UnityWebRequest unityWebRequest;

        /// <summary>
        /// 单位资源的百分比比率；
        /// </summary>
        float UnitResRatio { get { return 100f / downloadCount; } }
        /// <summary>
        /// 当前下载的序号；
        /// </summary>
        int currentDownloadIndex = 0;
        /// <summary>
        /// 当前是否可下载；
        /// </summary>
        bool canDownload;
        /// <summary>
        /// 下载任务数量；
        /// </summary>
        int downloadCount = 0;
        public void InitDownloader(string url, string downloadPath)
        {
            this.URL = url;
            this.DownloadPath = downloadPath;
        }
        /// <summary>
        /// 移除下载文件；
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void RemoveDownloadFile(string fileName)
        {
            if (pendingURIs.Remove(fileName))
                downloadCount--;
        }
        /// <summary>
        /// 添加下载文件；
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void AddDownloadFile(string fileName)
        {
            pendingURIs.Add(fileName);
            downloadCount++;
        }
        /// <summary>
        /// 添加多个下载文件
        /// </summary>
        /// <param name="fileList">文件列表</param>
        public void AddDownloadFiles(string[] fileList)
        {
            pendingURIs.AddRange(fileList);
            downloadCount += fileList.Length;
        }
        /// <summary>
        /// 启动下载；
        /// </summary>
        public void LaunchDownload()
        {
            canDownload = true;
            if (pendingURIs.Count == 0 || !canDownload)
            {
                canDownload = false;
                return;
            }
            Downloading = true;
            downloadStartTime = DateTime.Now;
            Utility.Unity.StartCoroutine(DownloadMultipleFiles());
        }
        /// <summary>
        /// 移除所有下载；
        /// </summary>
        public void RemoveAllDownload()
        {
            OnCancelDownload();
        }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        public void CancelDownload()
        {
            OnCancelDownload();
        }
        public void Release()
        {
            downloadStart = null;
            downloadSuccess = null;
            downloadFailure = null;
            downloadOverall = null;
            downloadFinish = null;
            downloadCount = 0;
        }
        IEnumerator DownloadMultipleFiles()
        {
            while (pendingURIs.Count > 0)
            {
                var uri = pendingURIs.RemoveFirst();
                currentDownloadIndex = downloadCount - pendingURIs.Count - 1;
                var fileDownloadPath = Path.Combine(DownloadPath, uri);
                var remoteUri = Utility.IO.WebPathCombine(URL, uri);
                yield return DownloadSingleFile(remoteUri, fileDownloadPath);
            }
            OnDownloadedPendingFiles();
        }
        IEnumerator DownloadSingleFile(string uri, string downloadPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                unityWebRequest = request;
                var timeout = Convert.ToInt32(DownloadTimeout);
                if (timeout > 0)
                    request.timeout = timeout;
                downloadStart?.Invoke(uri, downloadPath);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    OnFileDownloading(uri, DownloadPath, request.downloadProgress);
                    var responseData = new ResponseData(uri, request.downloadHandler.data, downloadPath);
                    yield return OnDownloadedData(responseData);
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var responseData = new ResponseData(uri, request.downloadHandler.data, downloadPath);
                        yield return OnDownloadedData(responseData);
                        downloadSuccess?.Invoke(uri, downloadPath, request.downloadHandler.data);
                        OnFileDownloading(uri, DownloadPath, 1);
                        successURIs.Add(uri);
                    }
                }
                else
                {
                    Downloading = false;
                    downloadFailure?.Invoke(request.url, downloadPath, request.error);
                    failureURIs.Add(uri);
                    OnFileDownloading(uri, DownloadPath, 1);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(downloadPath);
                    }
                }
            }
        }
        Task OnDownloadedData(ResponseData responseData)
        {
            var cachedLenth = responseData.Data.Length;
            if (dataWriteDict.TryGetValue(responseData.URI, out var writeInfo))
            {
                if (writeInfo.CachedLength >= cachedLenth)
                    return null;
                //缓存新数据的长度，保留原来写入的长度；
                dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, writeInfo.WrittenLength));
                return Task.Run(() =>
                {
                    Utility.IO.WriteFile(responseData.Data, responseData.DownloadPath);
                    dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, cachedLenth));
                });
            }
            else
            {
                dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, 0));
                return Task.Run(() =>
                {
                    Utility.IO.WriteFile(responseData.Data, responseData.DownloadPath);
                    dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, cachedLenth));
                });
            }
        }
        //IEnumerator OnDownloadedData(ResponseData responseData)
        //{
        //    var cachedLenth = responseData.Data.Length;
        //    if (dataWriteDict.TryGetValue(responseData.URI, out var writeInfo))
        //    {
        //        if (writeInfo.CachedLength < cachedLenth)
        //        {
        //            dataCacheDict[responseData.URI] = responseData;
        //            //缓存新数据的长度，保留原来写入的长度；
        //            dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, writeInfo.WrittenLength));
        //            Utility.IO.WriteFile(responseData.Data, responseData.DownloadPath);
        //            dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, cachedLenth));
        //        }
        //    }
        //    else
        //    {
        //        dataCacheDict.Add(responseData.URI, responseData);
        //        dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, 0));
        //        Utility.IO.WriteFile(responseData.Data, responseData.DownloadPath);
        //        dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, cachedLenth));
        //    }
        //    yield return null;
        //}
        /// <summary>
        /// 处理整体进度；
        /// individualPercent 0~1；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="individualPercent">资源个体百分比0~1</param>
        void OnFileDownloading(string uri, string downloadPath, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / downloadCount);
            var overallProgress = overallIndexPercent + (UnitResRatio * individualPercent);
            downloadOverall.Invoke(uri, downloadPath, overallProgress, individualPercent * 100);
        }
        void OnDownloadedPendingFiles()
        {
            canDownload = false;
            Downloading = false;
            downloadEndTime = DateTime.Now;
            downloadFinish?.Invoke(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            pendingURIs.Clear();
            failureURIs.Clear();
            successURIs.Clear();
            downloadCount = 0;
        }
        void OnCancelDownload()
        {
            unityWebRequest?.Abort();
            downloadCount = 0;
            pendingURIs.Clear();
            failureURIs.Clear();
            successURIs.Clear();
            canDownload = false;
        }
    }
}
