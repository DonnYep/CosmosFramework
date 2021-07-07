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
        /// <summary>
        /// 是否正在下载；
        /// </summary>
        public bool Downloading { get; private set; }
        /// <summary>
        /// 可下载的资源总数；
        /// </summary>
        public int DownloadableCount { get; private set; }

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

        QuarkDownloadConfig downloadConfig;
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
        public void SetDownloadConfig(QuarkDownloadConfig downloadConfig)
        {
            this.downloadConfig = downloadConfig;
            pendingURIs.AddRange(downloadConfig.FileList);
            DownloadableCount = downloadConfig.FileList.Length;
            unitResRatio = 100f / DownloadableCount;
        }
        /// <summary>
        /// 启动下载；
        /// </summary>
        public void LaunchDownload()
        {
            canDownload = true;
            if (pendingURIs.Count == 0 || !canDownload)
                return;
            Downloading = true;
            downloadStartTime = DateTime.Now;
            //RecursiveDownload();
            Utility.Unity.StartCoroutine(EnumWebRequest());
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
        public void Release()
        {
            downloadStart = null;
            downloadSuccess = null;
            downloadFailure = null;
            downloadOverall = null;
            downloadFinish = null;
            downloadConfig.Reset();
            DownloadableCount = 0;
        }
        IEnumerator EnumWebRequest()
        {
            var length = pendingURIs.Count;
            for (int i = 0; i < length; i++)
            {
                currentDownloadIndex = i;
                var fileDownloadPath = Path.Combine(downloadConfig.DownloadPath, pendingURIs[i]);
                var remoteUri = Utility.IO.WebPathCombine(downloadConfig.URL, pendingURIs[i]);
                yield return DownloadWebRequest(remoteUri, fileDownloadPath);
            }
            canDownload = false;
            Downloading = false;
            downloadEndTime = DateTime.Now;
            downloadFinish?.Invoke(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
        }
        IEnumerator DownloadWebRequest(string uri, string fileDownloadPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                unityWebRequest = request;
                var timeout = Convert.ToInt32(downloadConfig.DownloadTimeout);
                if (timeout > 0)
                    request.timeout = timeout;
                downloadStart?.Invoke(uri, fileDownloadPath);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    ProcessOverallProgress(uri, downloadConfig.DownloadPath, request.downloadProgress);
                    var responseData = new ResponseData(uri, request.downloadHandler.data, fileDownloadPath);
                    yield return WriteResponseData(responseData);
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var responseData = new ResponseData(uri, request.downloadHandler.data, fileDownloadPath);
                        yield return WriteResponseData(responseData);
                        downloadSuccess?.Invoke(uri, fileDownloadPath, request.downloadHandler.data);
                        ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                        successURIs.Add(uri);
                    }
                }
                else
                {
                    Downloading = false;
                    downloadFailure?.Invoke(request.url, fileDownloadPath, request.error);
                    failureURIs.Add(uri);
                    ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                    if (downloadConfig.DeleteFailureFile)
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
        Task WriteResponseData(ResponseData responseData)
        {
            var cachedLenth = responseData.Data.Length;
            if (dataWriteDict.TryGetValue(responseData.URI, out var writeInfo))
            {
                if (writeInfo.CachedLength >= cachedLenth)
                    return null;
                dataCacheDict[responseData.URI] = responseData;
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
                dataCacheDict.Add(responseData.URI, responseData);
                dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, 0));
                return Task.Run(() =>
                {
                    Utility.IO.WriteFile(responseData.Data, responseData.DownloadPath);
                    dataWriteDict.AddOrUpdate(responseData.URI, new ResponseWriteInfo(cachedLenth, cachedLenth));
                });
            }
        }
        //IEnumerator WriteResponseData(ResponseData responseData)
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
    }
}
