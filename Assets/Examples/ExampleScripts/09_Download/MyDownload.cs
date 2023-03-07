using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
using System;

public class MyDownload : MonoBehaviour
{
    [SerializeField] string srcUrl;
    [Header("文件下载到的绝对路径")]
    [SerializeField] string downloadPath;
    [SerializeField] Slider slider;
    [SerializeField] Text text;
    [SerializeField] Text uriText;
    int downloadTaskId;
    void Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        CosmosEntry.DownloadManager.OnDownloadSuccess += OnDownloadSucess;
        CosmosEntry.DownloadManager.OnDownloadFailure += OnDownloadFailure;
        CosmosEntry.DownloadManager.OnDownloadStart += OnDownloadStart;
        CosmosEntry.DownloadManager.OnDownloadOverallProgress += OnDownloadOverall;
        CosmosEntry.DownloadManager.OnAllDownloadTaskCompleted += OnDownloadFinish;
        downloadTaskId = CosmosEntry.DownloadManager.AddDownload(srcUrl, downloadPath);
        CosmosEntry.DownloadManager.LaunchDownload();
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.DownloadInfo.DownloadUri;
    }
    void OnDownloadOverall(DonwloadUpdateEventArgs eventArgs)
    {
        var progress = eventArgs.CurrentDownloadTaskIndex / (float)eventArgs.DownloadTaskCount;
        var overallProgress = (float)Math.Round(progress, 1);
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
        if (eventArgs.DownloadInfo.DownloadId == downloadTaskId)
            Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.DownloadInfo.DownloadUri}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        if (eventArgs.DownloadInfo.DownloadId == downloadTaskId)
            Utility.Debug.LogError($"DownloadFailure {eventArgs.DownloadInfo.DownloadUri}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadTasksCompletedEventArgs eventArgs)
    {
        if (text != null)
        {
            text.text = "100%   Done";
        }
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.TimeSpan}", DebugColor.green);
    }
}
