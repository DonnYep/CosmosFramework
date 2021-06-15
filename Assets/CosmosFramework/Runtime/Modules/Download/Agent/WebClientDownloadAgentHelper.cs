using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;

namespace Cosmos.Download
{
    public class WebClientDownloadAgentHelper : IDownloadAgentHelper
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadUpdateEventArgs> downloadUpdate;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadUpdateEventArgs> DownloadUpdate
        {
            add { downloadUpdate += value; }
            remove { downloadUpdate -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadSuccess
        {
            add { downloadSuccess += value; }
            remove { downloadSuccess -= value; }
        }
        public event Action<DownloadFailureEventArgs> DownloadFailure
        {
            add { downloadFailure += value; }
            remove { downloadFailure -= value; }
        }
        public IEnumerator DownloadFileAsync(DownloadTask downloadTask, object customeData)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    var startEventArgs = DownloadStartEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, customeData);
                    downloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);

                    Task task = webClient.DownloadDataTaskAsync(downloadTask.Uri);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        var progress = eventArgs.ProgressPercentage;
                        var updateEventArgs = DownloadUpdateEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, progress, customeData);
                        downloadUpdate?.Invoke(updateEventArgs);
                        DownloadUpdateEventArgs.Release(updateEventArgs);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        var successEventArgs = DownloadSuccessEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, eventArgs.Result, customeData);
                        downloadSuccess?.Invoke(successEventArgs);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                    };
                    return task.AsCoroutine();
                }
                catch (Exception exception)
                {
                    var failureEventArgs = DownloadFailureEventArgs.Create(downloadTask.Uri, null, exception.ToString(), customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    throw exception;
                }
            }
        }
        public IEnumerator DownloadFileAsync(DownloadTask downloadTask, long startPosition, object customeData)
        {
            return null;
        }
    }
}
