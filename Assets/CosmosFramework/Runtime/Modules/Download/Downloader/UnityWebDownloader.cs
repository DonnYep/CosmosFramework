using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Cosmos.Download
{
    public class UnityWebDownloader : Downloader
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
                var timeout = Convert.ToInt32(downloadConfig.DownloadTimeout);
                if (timeout > 0)
                    request.timeout = timeout;
                var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                downloadStart?.Invoke(startEventArgs);
                DownloadStartEventArgs.Release(startEventArgs);
                var operation = request.SendWebRequest();
                while (!operation.isDone && canDownload)
                {
                    ProcessOverallProgress(uri, downloadConfig.DownloadPath, request.downloadProgress);
                    var downloadedData = new DownloadedData(uri, request.downloadHandler.data, fileDownloadPath);
                    CacheDownloadedData(downloadedData);
                    yield return null;
                }
                if (!request.isNetworkError && !request.isHttpError && canDownload)
                {
                    if (request.isDone)
                    {
                        Downloading = false;
                        var downloadData = request.downloadHandler.data;
                        var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, downloadData);
                        downloadSuccess?.Invoke(successEventArgs);
                        ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successURIs.Add(uri);
                        var downloadedData = new DownloadedData(uri, request.downloadHandler.data, fileDownloadPath);
                        CacheDownloadedData(downloadedData);
                    }
                }
                else
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, request.error);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
                    ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                    if (downloadConfig.DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
            }
        }
    }
}
