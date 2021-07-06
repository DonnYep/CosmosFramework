using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
using System;


public class DownloadTest : MonoBehaviour
{
    [SerializeField]
    string srcUrl;
    [SerializeField]
    string downloadPath;
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text text;
    [SerializeField]
    Text uriText;
    int srcCount;
    void  Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        var len = srcUrl.Length;
        List<string> downloadableUri = new List<string>();
        var fileList = new List<string>();
        if (Directory.Exists(srcUrl))
        {
            Utility.IO.TraverseFolderFile(srcUrl, (info) =>
            {
                var path = info.FullName;
                var name = info.FullName.Remove(0, len + 1);
                downloadableUri.Add(name);
            });
        }
        else
        {
            var result= Utility.Net.PingURI(srcUrl);
            if (result)
            {
                Utility.Net.PingUrlFileList(srcUrl, fileList);
                var length = fileList.Count;
                for (int i = 0; i < length; i++)
                {
                    var uri = fileList[i].Remove(0, len);
                    downloadableUri.Add(uri);
                    Utility.Debug.LogInfo(uri, MessageColor.YELLOW);
                }
            }
        }
        srcCount = downloadableUri.Count;
        if (srcCount > 0)
        {
            var cfg = new DownloadConfig(srcUrl, downloadPath, downloadableUri.ToArray());
            CosmosEntry.DownloadManager.DownloadSuccess += OnDownloadSucess;
            CosmosEntry.DownloadManager.DownloadFailure += OnDownloadFailure;
            CosmosEntry.DownloadManager.DownloadStart += OnDownloadStart;
            CosmosEntry.DownloadManager.DownloadOverall += OnDownloadOverall;
            CosmosEntry.DownloadManager.DownloadAndWriteFinish+= OnDownloadFinish;
            CosmosEntry.DownloadManager.SetDownloadConfig(cfg);
            CosmosEntry.DownloadManager.LaunchDownload();
        }
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.URI;
    }
    void OnDownloadOverall(DonwloadOverallEventArgs eventArgs)
    {
        var overallProgress = (float)Math.Round(eventArgs.OverallProgress, 1);
        if (text != null)
        {
            text.text = overallProgress + "%";
        }
        if (slider != null)
        {
            slider.value = overallProgress;
        }
    }
    void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
    {
      // Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.URI}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.URI}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadAndWriteFinishEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.DownloadAndWriteTimeSpan}",MessageColor.GREEN);
    }
}
