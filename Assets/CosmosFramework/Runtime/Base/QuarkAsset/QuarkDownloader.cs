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
        /// URL---DownloadPath---OverallProgress---IndividualProgress
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

        Dictionary<string, ResponseData> dataCacheDict = new Dictionary<string, ResponseData>();

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
        /// 是否可写入本地；
        /// </summary>
        bool canWrite;
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
            canWrite = true;
            downloadStartTime = DateTime.Now;
            RecursiveDownload();
        }
        /// <summary>
        /// 下载轮询，需要由外部调用；
        /// </summary>
        public async void TickRefresh()
        {
            if (!canWrite && !canDownload)
                return;
            if (dataCacheDict.Count > 0)
            {
                var data = dataCacheDict.First().Value;
                dataCacheDict.Remove(data.URI);
                if (dataWriteDict.TryGetValue(data.URI, out var writeInfo))
                {
                    if (writeInfo.CachedLength > writeInfo.WrittenLength)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                Utility.IO.WriteFile(data.Data, data.DownloadPath);
                                dataWriteDict.AddOrUpdate(data.URI, new ResponseWriteInfo(writeInfo.CachedLength, writeInfo.CachedLength));
                            }
                            catch { }
                        });
                    }
                }
                canWrite = dataCacheDict.Count > 0 ? true : false;
            }
        }
        /// <summary>
        /// 终止下载，谨慎使用；
        /// </summary>
        public void CancelDownload()
        {
            failureURIs.AddRange(pendingURIs);
            pendingURIs.Clear();
            downloadFinish?.Invoke(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
            unityWebRequest?.Abort();
            canDownload = false;
        }
        public virtual void Reset()
        {
            downloadStart = null;
            downloadSuccess = null;
            downloadFailure = null;
            downloadOverall = null;
            downloadFinish = null;
            DownloadableCount = 0;
            canWrite = false;
        }
        async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                downloadEndTime = DateTime.Now;
                downloadFinish?.Invoke(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
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
                downloadStart?.Invoke(request.url, fileDownloadPath);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    ProcessOverallProgress(uri, DownloadPath, request.downloadProgress);
                    var responseData = new ResponseData(uri, request.downloadHandler.data, fileDownloadPath);
                    CacheResponseData(responseData);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        downloadSuccess?.Invoke(request.url, fileDownloadPath, request.downloadHandler.data);
                        ProcessOverallProgress(uri, DownloadPath, 1);
                        successURIs.Add(uri);
                        var responseData = new ResponseData(uri, request.downloadHandler.data, fileDownloadPath);
                        CacheResponseData(responseData);
                    }
                }
                else
                {
                    Downloading = false;
                    downloadFailure?.Invoke(request.url, fileDownloadPath, request.error);
                    failureURIs.Add(uri);
                    ProcessOverallProgress(uri, DownloadPath, 1);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }
        }
        /// <summary>
        /// 处理整体进度；
        /// individualPercent 0~1；
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="downloadPath">下载到本地的目录</param>
        /// <param name="individualPercent">资源个体百分比0~1</param>
        void ProcessOverallProgress(string uri, string downloadPath, float individualPercent)
        {
            var overallIndexPercent = 100 * ((float)currentDownloadIndex / DownloadableCount);
            var overallProgress = overallIndexPercent + (unitResRatio * individualPercent);
            downloadOverall.Invoke(uri, downloadPath, overallProgress, individualPercent * 100);
        }
        void CacheResponseData(ResponseData responseData)
        {
            if (dataCacheDict.TryGetValue(responseData.URI, out var data))
            {
                if (data.Data.Length < responseData.Data.Length)
                {
                    dataCacheDict[responseData.URI] = responseData;
                    //缓存新数据的长度，保留原来写入的长度；
                    dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(responseData.Data.Length, data.Data.Length));
                }
            }
            else
            {
                if (!successURIs.Contains(responseData.URI))
                {
                    dataWriteDict.TryGetValue(responseData.URI, out var writeInfo);
                    //旧缓存的长度小于新缓存的长度，则更换
                    if (writeInfo.CachedLength < responseData.Data.Length)
                    {
                        dataCacheDict.Add(responseData.URI, responseData);
                        dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(responseData.Data.Length, 0));
                    }
                }
            }
        }

    }
}
