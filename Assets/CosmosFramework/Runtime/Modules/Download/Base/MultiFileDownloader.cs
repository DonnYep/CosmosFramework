using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Cosmos.Download
{
    public class MultiFileDownloader
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadUpdateEventArgs> downloadUpdate;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        List<string> pendingURIs = new List<string>();
        List<string> completedURIs = new List<string>();
        List<string> failureURIs = new List<string>();
        string downloadPath;
        bool downloading;
        public string DownloadPath { get { return downloadPath; } set { downloadPath = value; } }
        public bool Downloading { get { return downloading; } set { downloading = value; } }
        DateTime startTime;
        int timeout;
        public bool AbortOnFailure { get; set; }
        public bool Download()
        {
            if (pendingURIs.Count == 0 || downloading)
                return false;
            downloading = true;
            startTime = DateTime.Now;
            RecursiveDownload();
            return true;
        }
        async void RecursiveDownload()
        {
            if (pendingURIs.Count == 0)
            {
                CancelDownload(null);
                return;
            }
            string uri = pendingURIs[0];
            var fileName = URI2FileName(uri);
            var fileDownloadPath = Path.Combine(downloadPath, fileName);
            pendingURIs.RemoveAt(0);
            if (!downloading)
            {
                CancelDownload(uri);
            }
            if (!AbortOnFailure)
            {
                await DownloadWebRequest(uri, fileDownloadPath, null);
                RecursiveDownload();
            }
        }
        async Task DownloadWebRequest(string uri, string fileDownloadPath, object customeData)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, fileDownloadPath, customeData);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                await request.SendWebRequest();
                while (!request.isDone)
                {
                    var percentage = (int)request.downloadProgress * 100;
                    var updateEventArgs = DownloadUpdateEventArgs.Create(request.url, fileDownloadPath, percentage, customeData);
                    downloadUpdate?.Invoke(updateEventArgs);
                    DownloadUpdateEventArgs.Release(updateEventArgs);
                }
                if (!request.isNetworkError && !request.isHttpError)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var updateEventArgs = DownloadUpdateEventArgs.Create(request.url, fileDownloadPath, 100, customeData);
                        downloadUpdate?.Invoke(updateEventArgs);
                        var successEventArgs = DownloadSuccessEventArgs.Create(request.url, fileDownloadPath, request.downloadHandler.data, customeData);
                        downloadSuccess?.Invoke(successEventArgs);
                        DownloadUpdateEventArgs.Release(updateEventArgs);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        completedURIs.Add(uri);
                        Utility.IO.WriteFile(request.downloadHandler.data, fileDownloadPath);
                    }
                }
                else
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(request.url, fileDownloadPath, request.error, customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
                }
            }
        }
        void CancelDownload(string uri)
        {

        }
        string URI2FileName(string uri)
        {
            return string.Empty;
        }
    }
}
