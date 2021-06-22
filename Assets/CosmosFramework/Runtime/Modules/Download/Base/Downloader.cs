using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class Downloader
    {
        IDownloadHelper downloadHelper;

        List<DownloadInfo> pendingInfos;
        List<DownloadInfo> completedInfos;
        List<DownloadInfo> failureInfos;

        public List<DownloadInfo> PendingInfos { get { return pendingInfos; } }
        public List<DownloadInfo> CompletedInfos { get { return completedInfos; } }
        public List<DownloadInfo> FailureInfos { get { return failureInfos; } }

        public bool Downloading { get { return downloadHelper.Downloading; } }

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
        public Downloader(IDownloadHelper helper) 
        {
            pendingInfos = new List<DownloadInfo>();
            completedInfos = new List<DownloadInfo>();
            failureInfos = new List<DownloadInfo>();
            this.downloadHelper = helper;
            downloadHelper.DownloadFailure += downloadFailure;
            downloadHelper.DownloadStart += downloadStart;
            downloadHelper.DownloadSuccess += downloadSuccess;
            downloadHelper.DownloadUpdate += downloadUpdate;
        }
        public void Download()
        {
            if (!Downloading || pendingInfos.Count >= 0)
            {
                if (downloadHelper == null)
                    throw new ArgumentNullException("DownloadHelper is invalid ! ");
                var downloadInfo = pendingInfos[0];
                DownloadFilesAsync(downloadInfo);
            }
        }
        public void CancelDownload(DownloadInfo downloadInfo)
        {

        }
        public void AbortDownloader()
        {

            downloadHelper.ClearEvents();
        }
        async void DownloadFilesAsync(DownloadInfo downloadInfo)
        {
            await downloadHelper.DownloadFileAsync(downloadInfo, null);
            pendingInfos.RemoveAt(0);
            if (pendingInfos.Count > 0)
            {
                var info = pendingInfos[0];
                DownloadFilesAsync(info);
            }
        }
        [TickRefresh]
        void TickRefresh()
        {
            var length = pendingInfos.Count;
            for (int i = 0; i < length; i++)
            {
                var info = pendingInfos[i];
            }
        }
    }
}
