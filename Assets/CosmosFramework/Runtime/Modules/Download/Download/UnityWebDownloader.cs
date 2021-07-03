using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Cosmos.Download
{
    public class UnityWebDownloader: Downloader
    {
        UnityWebRequest unityWebRequest;
        protected override void CancelWebAsync()
        {
            unityWebRequest?.Abort();
        }
        protected override IEnumerator WebDownload(string uri, string fileDownloadPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                Downloading = true;
                unityWebRequest = request;
                var timeout = Convert.ToInt32(DownloadTimeout);
                if (timeout> 0)
                    request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(request.url, fileDownloadPath);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    ProcessOverallProgress(uri, DownloadPath, request.downloadProgress);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var successEventArgs = DownloadSuccessEventArgs.Create(request.url, fileDownloadPath, request.downloadHandler.data);
                        downloadSuccess?.Invoke(successEventArgs);
                        ProcessOverallProgress(uri, DownloadPath, 1);
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
                    ProcessOverallProgress(uri, DownloadPath, 1);
                    if (DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }
        }
    }
}
