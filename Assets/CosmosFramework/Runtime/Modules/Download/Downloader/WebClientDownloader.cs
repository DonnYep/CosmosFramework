using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class WebClientDownloader : Downloader
    {
        WebClient webClient;
        protected override void CancelWebAsync()
        {
            webClient?.CancelAsync();
        }
        protected override IEnumerator WebDownload(string uri, string fileDownloadPath)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Downloading = true;
                    this.webClient = webClient;
                    var startEventArgs = DownloadStartEventArgs.Create(uri, fileDownloadPath);
                    downloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        ProcessOverallProgress(uri, downloadConfig.DownloadPath, (float)eventArgs.ProgressPercentage / 100);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        Downloading = false;
                        var resultData = eventArgs.Result;
                        var successEventArgs = DownloadSuccessEventArgs.Create(uri, fileDownloadPath, resultData);
                        downloadSuccess?.Invoke(successEventArgs);
                        ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                        successURIs.Add(uri);
                        var downloadedData = new DownloadedData(uri, resultData, fileDownloadPath);
                        WriteDownloadedData(downloadedData);
                    };
                }
                catch (Exception exception)
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(uri, fileDownloadPath, exception.Message);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    failureURIs.Add(uri);
                    ProcessOverallProgress(uri, downloadConfig.DownloadPath, 1);
                    if (downloadConfig.DeleteFailureFile)
                    {
                        Utility.IO.DeleteFile(fileDownloadPath);
                    }
                }
                yield return webClient.DownloadDataTaskAsync(uri);
            }
        }
    }
}
