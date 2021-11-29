using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Quark.Asset;
using Utility = Cosmos.Utility;
using UnityEngine;
using Cosmos;

namespace Quark.Networking
{
    /// <summary>
    /// Quark资源下载器；
    /// 资源被下载到本地持久化路径后，再由Qurk加载器进行资源加载；
    /// </summary>
    public class QuarkDownloader
    {
        /// <summary>
        /// 网络下载反馈的数据；
        /// </summary>
        class DownloadedData
        {
            public DownloadedData(string uri, byte[] data, string downloadPath)
            {
                URI = uri;
                Data = data;
                DownloadPath = downloadPath;
            }
            public string URI { get; private set; }
            public byte[] Data { get; private set; }
            public string DownloadPath { get; private set; }
        }
        class DataWrittenInfo
        {
            /// <summary>
            /// 已经缓存的长度；
            /// </summary>
            public long CachedLength { get; private set; }
            /// <summary>
            /// 已经写入持久化本地的长度；
            /// </summary>
            public long WrittenLength { get; private set; }
            public DataWrittenInfo(long cachedLength, long writtenLength)
            {
                CachedLength = cachedLength;
                WrittenLength = writtenLength;
            }
        }
        #region events
        Action<string, string> onDownloadStart;
        Action<string, string, byte[]> onDownloadSuccess;
        Action<string, string, string> onDownloadFailure;
        Action<string, string, float, float> onDownloadOverall;
        Action<string[], string[], TimeSpan> onDownloadFinish;
        /// <summary>
        /// URL---DownloadPath
        /// </summary>
        public event Action<string, string> OnDownloadStart
        {
            add { onDownloadStart += value; }
            remove { onDownloadStart -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---Data
        /// </summary>
        public event Action<string, string, byte[]> OnDownloadSuccess
        {
            add { onDownloadSuccess += value; }
            remove { onDownloadSuccess -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---ErrorMessage
        /// </summary>
        public event Action<string, string, string> OnDownloadFailure
        {
            add { onDownloadFailure += value; }
            remove { onDownloadFailure -= value; }
        }
        /// <summary>
        /// URL---DownloadPath---OverallProgress(0~100%)---IndividualProgress(0~100%)
        /// </summary>
        public event Action<string, string, float, float> OnDownloadOverall
        {
            add { onDownloadOverall += value; }
            remove { onDownloadOverall -= value; }
        }
        /// <summary>
        /// SuccessURIs---FailureURIs---TimeSpan
        /// </summary>
        public event Action<string[], string[], TimeSpan> OnDownloadFinish
        {
            add { onDownloadFinish += value; }
            remove { onDownloadFinish -= value; }
        }
        #endregion
        public string PersistentPath { get { return QuarkDataProxy.PersistentPath; } }
        public string URL { get { return QuarkDataProxy.URL; } }
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
        Dictionary<string, DataWrittenInfo> dataWriteDict = new Dictionary<string, DataWrittenInfo>();
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
            QuarkUtility.Unity.StartCoroutine(EnumDownloadMultipleFiles());
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
            onDownloadStart = null;
            onDownloadSuccess = null;
            onDownloadFailure = null;
            onDownloadOverall = null;
            onDownloadFinish = null;
            downloadCount = 0;
        }
        IEnumerator EnumDownloadMultipleFiles()
        {
            while (pendingURIs.Count > 0)
            {
                var uri = pendingURIs.RemoveFirst();
                currentDownloadIndex = downloadCount - pendingURIs.Count - 1;
                var fileDownloadPath = Path.Combine(PersistentPath, uri);
                var remoteUri = Utility.IO.WebPathCombine(URL, uri);
                yield return EnumDownloadSingleFile(remoteUri, fileDownloadPath);
            }
            OnDownloadedPendingFiles();
        }
        IEnumerator EnumDownloadSingleFile(string uri, string downloadPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                unityWebRequest = request;
                var timeout = Convert.ToInt32(DownloadTimeout);
                if (timeout > 0)
                    request.timeout = timeout;
                onDownloadStart?.Invoke(uri, downloadPath);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    OnFileDownloading(uri, PersistentPath, request.downloadProgress);
                    var responseData = new DownloadedData(uri, request.downloadHandler.data, downloadPath);
                    yield return OnDownloadedData(responseData);
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var responseData = new DownloadedData(uri, request.downloadHandler.data, downloadPath);
                        yield return OnDownloadedData(responseData);
                        onDownloadSuccess?.Invoke(uri, downloadPath, request.downloadHandler.data);
                        OnFileDownloading(uri, PersistentPath, 1);
                        successURIs.Add(uri);
                    }
                }
                else
                {
                    Downloading = false;
                    onDownloadFailure?.Invoke(request.url, downloadPath, request.error);
                    failureURIs.Add(uri);
                    OnFileDownloading(uri, PersistentPath, 1);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(downloadPath);
                    }
                }
            }
        }
        //Task OnDownloadedData(DownloadedData downloadedData)
        //{
        //    var cachedLenth = downloadedData.Data.Length;
        //    if (dataWriteDict.TryGetValue(downloadedData.URI, out var writeInfo))
        //    {
        //        if (writeInfo.CachedLength >= cachedLenth)
        //            return null;
        //        //缓存新数据的长度，保留原来写入的长度；
        //        dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, writeInfo.WrittenLength));
        //        return Task.Run(() =>
        //        {
        //            Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
        //            dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, cachedLenth));
        //        });
        //    }
        //    else
        //    {
        //        dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, 0));
        //        return Task.Run(() =>
        //        {
        //            Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
        //            dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, cachedLenth));
        //        });
        //    }
        //}
        IEnumerator OnDownloadedData(DownloadedData downloadedData)
        {
            var cachedLenth = downloadedData.Data.Length;
            if (dataWriteDict.TryGetValue(downloadedData.URI, out var writeInfo))
            {
                if (writeInfo.CachedLength < cachedLenth)
                {
                    //缓存新数据的长度，保留原来写入的长度；
                    dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, writeInfo.WrittenLength));
                    Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                    dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, cachedLenth));
                }
            }
            else
            {
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, 0));
                Utility.IO.WriteFile(downloadedData.Data, downloadedData.DownloadPath);
                dataWriteDict.AddOrUpdate(downloadedData.URI, new DataWrittenInfo(cachedLenth, cachedLenth));
            }
            yield return new WaitForSeconds(0.1f);
        }
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
            onDownloadOverall.Invoke(uri, downloadPath, overallProgress, individualPercent * 100);
        }
        void OnDownloadedPendingFiles()
        {
            canDownload = false;
            Downloading = false;
            downloadEndTime = DateTime.Now;
            onDownloadFinish?.Invoke(successURIs.ToArray(), failureURIs.ToArray(), downloadEndTime - downloadStartTime);
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
