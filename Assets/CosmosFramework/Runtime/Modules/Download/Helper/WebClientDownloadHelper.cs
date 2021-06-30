using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;

namespace Cosmos.Download
{
    public class WebClientDownloadHelper //: IDownloadHelper
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
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
        public bool Downloading { get; private set; }

        public Task DownloadFileAsync(DownloadInfo downloadTask, object customeData)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Downloading = true;
                    var startEventArgs = DownloadStartEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, customeData);
                    downloadStart?.Invoke(startEventArgs);
                    DownloadStartEventArgs.Release(startEventArgs);

                    Task task = webClient.DownloadDataTaskAsync(downloadTask.Uri);
                    webClient.DownloadProgressChanged += (sender, eventArgs) =>
                    {
                        var progress = eventArgs.ProgressPercentage;
                        //var updateEventArgs = DownloadIndividualEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, progress, customeData);
                        //downloadUpdate?.Invoke(updateEventArgs);
                        //DownloadIndividualEventArgs.Release(updateEventArgs);
                    };
                    webClient.DownloadDataCompleted += (sender, eventArgs) =>
                    {
                        Downloading = true;
                        var successEventArgs = DownloadSuccessEventArgs.Create(downloadTask.Uri, downloadTask.DownloadPath, eventArgs.Result, customeData);
                        downloadSuccess?.Invoke(successEventArgs);
                        DownloadSuccessEventArgs.Release(successEventArgs);
                    };
                    return task;
                }
                catch (Exception exception)
                {
                    Downloading = false;
                    var failureEventArgs = DownloadFailureEventArgs.Create(downloadTask.Uri, null, exception.ToString(), customeData);
                    downloadFailure?.Invoke(failureEventArgs);
                    DownloadFailureEventArgs.Release(failureEventArgs);
                    throw exception;
                }
            }
        }
        public Task DownloadFileAsync(DownloadInfo downloadTask, long startPosition, object customeData)
        {
            return null;
        }

        public void CancelDownload()
        {

        }
    }
}
