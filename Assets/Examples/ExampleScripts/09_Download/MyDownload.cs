using UnityEngine;
using Cosmos.Download;
using Cosmos;
using UnityEngine.UI;
using System;

public class MyDownload : MonoBehaviour
{
    [Header("文件所在的Url根目录")]
    [SerializeField] string srcUrl;
    [Header("文件下载到的本地文件夹")]
    [SerializeField] string downloadPath;
    [SerializeField] Slider slider;
    [SerializeField] Text text;
    [SerializeField] Text uriText;
    long downloadTaskId;
    void Start()
    {
        CosmosEntry.DownloadManager.OnDownloadSuccess += OnDownloadSucess;
        CosmosEntry.DownloadManager.OnDownloadFailure += OnDownloadFailure;
        CosmosEntry.DownloadManager.OnDownloadStart += OnDownloadStart;
        CosmosEntry.DownloadManager.OnDownloadOverallProgress += OnDownloadOverall;
        CosmosEntry.DownloadManager.OnAllDownloadTaskCompleted += OnDownloadFinish;
        CosmosEntry.WebRequestManager.OnGetHtmlFilesFailureCallback += OnGetHtmlFilesFailureCallback;
        CosmosEntry.WebRequestManager.OnGetHtmlFilesSuccessCallback += OnGetHtmlFilesSuccessCallback;
        StartDownload();
    }
    void StartDownload()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        Utility.IO.EmptyFolder(downloadPath);
        CosmosEntry.WebRequestManager.AddUrlFileRequestTask(srcUrl);
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.DownloadInfo.DownloadUrl;
    }
    void OnDownloadOverall(DonwloadUpdateEventArgs eventArgs)
    {
        //Download模块支撑了cosmosframework的整个资源下载。Resource模块的assetbundle下载，其他模块的下载都依赖此模块。
        //部分下载任务本身仅知道下载连接，无法获取需要下载的二进制长度，因此无法获取总下载长度。所以，无法使用 【已经下载的长度 / 需要下载的长度】计算公式。
        var progress = eventArgs.CurrentDownloadTaskIndex / (float)eventArgs.DownloadTaskCount;
        var overallProgress = (float)Math.Round(progress, 1) * 100;
        Utility.Debug.LogInfo(overallProgress);
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
        Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.DownloadInfo.DownloadUrl}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadTasksCompletedEventArgs eventArgs)
    {
        if (text != null)
        {
            text.text = "100%   Done";
        }
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.TimeSpan}", DebugColor.green);
    }
    private void OnGetHtmlFilesSuccessCallback(Cosmos.WebRequest.WebRequestGetHtmlFilesSuccessEventArgs eventArgs)
    {
        var fileInfos = eventArgs.UrlFileInfos;
        foreach (var fileInfo in fileInfos)
        {
            var fileDownloadPath = Utility.Text.Combine(downloadPath, fileInfo.RelativeUrl, fileInfo.FileName);
            Utility.Debug.LogInfo(fileDownloadPath);
            CosmosEntry.DownloadManager.AddDownload(fileInfo.URL, fileDownloadPath);
        }
        CosmosEntry.DownloadManager.LaunchDownload();
    }
    private void OnGetHtmlFilesFailureCallback(Cosmos.WebRequest.WebRequestGetHtmlFilesFailureEventArgs eventArgs)
    {
        var errors = eventArgs.ErrorMessages;
        foreach (var error in errors)
        {
            Utility.Debug.LogError(error);
        }
    }
}
