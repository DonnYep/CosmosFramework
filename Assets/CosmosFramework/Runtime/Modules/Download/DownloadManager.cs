using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos.Download
{
    internal class DownloadManager : Module, IDownloadManager
    {
        Action<DownloadStartEventArgs> downloadStart;
        Action<DownloadSuccessEventArgs> downloadUpdate;
        Action<DownloadSuccessEventArgs> downloadSuccess;
        Action<DownloadFailureEventArgs> downloadFailure;
        public event Action<DownloadStartEventArgs> DownloadStart
        {
            add { downloadStart += value; }
            remove { downloadStart -= value; }
        }
        public event Action<DownloadSuccessEventArgs> DownloadUpdate
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

        IDownloadAgentHelper downloadAgentHelper;
        public async void DownLoadMultipleFiles(DownloadTasks task)
        {
            var uris = task.Uris;
            foreach (var uri in uris)
            {
                var t = DownloadTask.Create(uri, task.DownloadPath, task.Timeout);
               await downloadAgentHelper.DownloadFileAsync(null, null);
            }
        }
        public async void DownloadFileAsync(DownloadTask downloadTask, object customeData)
        {
           await downloadAgentHelper.DownloadFileAsync(downloadTask, customeData);
        }
    }
}
