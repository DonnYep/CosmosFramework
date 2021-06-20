using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Download
{
    public class Downloader
    {
        public bool Downloading { get; private set; }
        List<string> pendingURIs;
        List<string> completedURIs;
        List<string> downloadFailureURIs;
        public List<string> PendingURIs { get { return pendingURIs; } }
        public List<string> CompletedURIs { get { return completedURIs; } }
        public List<string> DownloadFailureURIs { get { return downloadFailureURIs; } }

        public string DownloadPath { get; private set; }
        public float DownloadTimeout { get; private set; }

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

        IDownloadHelper downloadHelper;

        public Downloader()
        {
            pendingURIs = new List<string>();
            completedURIs = new List<string>();
            downloadFailureURIs = new List<string>();
        }
        public async void SetDownloadHelper(IDownloadHelper helper)
        {
            if (downloadHelper != null && downloadHelper.Downloading)
            {
                await new UnityEngine.WaitUntil(() => downloadHelper.Downloading == false);
                downloadFailure = null;
                downloadStart = null;
                downloadSuccess = null;
                downloadUpdate = null;
                //downloadHelper.Release();
            }
            this.downloadHelper = helper;
            downloadHelper.DownloadFailure += downloadFailure;
            downloadHelper.DownloadStart+=downloadStart ;
            downloadHelper.DownloadSuccess+=downloadSuccess;
            downloadHelper.DownloadUpdate+=downloadUpdate;
        }
        public void Download()
        {
            if (!Downloading || pendingURIs.Count >= 0)
            {
                if (downloadHelper == null)
                    throw new ArgumentNullException("DownloadAgentHelper is invalid ! ");
                Downloading = true;
                DownloadFilesAsync();
            }
        }
        public async void DownloadFilesAsync()
        {
            var length = pendingURIs.Count;
            var downloadTasks = new List<DownloadInfo>();
            for (int i = 0; i < length; i++)
            {
                var downloadTask = new DownloadInfo (  pendingURIs[i] ,DownloadPath,DownloadTimeout);
                downloadTasks.Add(downloadTask);
            }
            for (int i = 0; i < length; i++)
            {
               // await downloadHelper.DownloadFileAsync(downloadTasks[i], null);
            }
        }
        public void TickRefresh()
        {

        }
    }
}
