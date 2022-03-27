using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
using System;

public class MyDownload : MonoBehaviour
{
    [SerializeField]string srcUrl;
    [Header("文件下载到的绝对路径")]
    [SerializeField]string downloadPath;
    [SerializeField]Slider slider;
    [SerializeField]Text text;
    [SerializeField]Text uriText;
    void  Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        CosmosEntry.DownloadManager.OnDownloadSuccess += OnDownloadSucess;
        CosmosEntry.DownloadManager.OnDownloadFailure += OnDownloadFailure;
        CosmosEntry.DownloadManager.OnDownloadStart += OnDownloadStart;
        CosmosEntry.DownloadManager.OnDownloadOverall += OnDownloadOverall;
        CosmosEntry.DownloadManager.OnDownloadAndWriteFinish += OnDownloadFinish;
        CosmosEntry.DownloadManager.AddUrlDownload(srcUrl, downloadPath);
        CosmosEntry.DownloadManager.LaunchDownload();
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
       Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.URI}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.URI}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadAndWriteFinishEventArgs eventArgs)
    {
        if (text != null)
        {
            text.text = "100%   Done";
        }
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.DownloadAndWriteTimeSpan}",MessageColor.GREEN);
    }
}
