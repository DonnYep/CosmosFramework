using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.Download
{
    /// <summary>
    /// 基于 <see cref="UnityWebRequest"/> 的下载传输实现。
    /// <para>要点：</para>
    /// <para>1、数据直接写入临时文件（<see cref="DownloadHandlerFile"/>），不占内存、中断可续；</para>
    /// <para>2、续传时带 <c>Range</c> 头，并保证 <c>removeFileOnAbort = false</c>（否则中断会把断点文件删掉）；</para>
    /// <para>3、总长度同时兼容 <c>Content-Length</c> 与 <c>Content-Range: bytes start-end/total</c>。</para>
    /// </summary>
    public sealed class UnityWebRequestDownloadTransport : IDownloadTransport
    {
        /// <inheritdoc/>
        public IDownloadTransfer CreateTransfer(DownloadTransferRequest request)
        {
            return new Transfer(request);
        }
        /// <summary>
        /// 单次传输
        /// </summary>
        sealed class Transfer : IDownloadTransfer
        {
            UnityWebRequest webRequest;
            DownloadTransferState state = DownloadTransferState.Running;
            long downloadedBytes;
            long totalBytes = -1L;
            int responseCode;
            string error;
            public Transfer(DownloadTransferRequest request)
            {
                try
                {
                    var unityWebRequest = new UnityWebRequest(request.DownloadUrl, UnityWebRequest.kHttpVerbGET);
                    //append 只在续传时为 true：DownloadHandlerFile 会以追加方式写临时文件
                    unityWebRequest.downloadHandler = new DownloadHandlerFile(request.TempPath, request.StartOffset > 0L)
                    {
                        //断点文件必须保留，否则中断后无法续传
                        removeFileOnAbort = false,
                    };
                    if (request.StartOffset > 0L)
                        unityWebRequest.SetRequestHeader("Range", "bytes=" + request.StartOffset + "-");
                    if (request.TimeoutSeconds > 0f)
                        unityWebRequest.timeout = Mathf.CeilToInt(request.TimeoutSeconds);
                    unityWebRequest.redirectLimit = 32;
                    unityWebRequest.SendWebRequest();
                    webRequest = unityWebRequest;
                }
                catch (Exception e)
                {
                    error = e.Message;
                    state = DownloadTransferState.Failed;
                }
            }
            public DownloadTransferState State { get { return state; } }
            public long DownloadedBytes { get { return downloadedBytes; } }
            public long TotalBytes { get { return totalBytes; } }
            public int ResponseCode { get { return responseCode; } }
            public string Error { get { return error; } }
            public void Tick()
            {
                var request = webRequest;
                if (request == null || state != DownloadTransferState.Running)
                    return;
                responseCode = (int)request.responseCode;
                downloadedBytes = (long)request.downloadedBytes;
                if (totalBytes < 0L)
                    totalBytes = Utility.Net.GetTransferLength(request);
                if (!request.isDone)
                    return;
                if (Utility.Net.IsRequestSucceeded(request))
                {
                    state = DownloadTransferState.Succeeded;
                }
                else
                {
                    error = string.IsNullOrEmpty(request.error) ? $"HTTP {responseCode}" : request.error;
                    state = DownloadTransferState.Failed;
                }
                DisposeRequest();
            }
            public void Cancel()
            {
                if (state != DownloadTransferState.Running)
                    return;
                state = DownloadTransferState.Canceled;
                try
                {
                    if (webRequest != null)
                        webRequest.Abort();
                }
                catch (Exception)
                {
                    //Abort 在请求已结束时可能抛，忽略即可
                }
                DisposeRequest();
            }
            void DisposeRequest()
            {
                if (webRequest == null)
                    return;
                try
                {
                    webRequest.Dispose();
                }
                catch (Exception)
                {
                }
                webRequest = null;
            }
        }
    }
}
